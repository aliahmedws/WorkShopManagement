import { Component, ElementRef, OnInit, ViewChild, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import { ListService, PagedResultDto } from '@abp/ng.core';
import { CheckListService } from '../proxy/check-lists';
import {
  commentTypeOptions,
  CreateListItemDto,
  GetListItemListDto,
  ListItemDto,
  ListItemService,
  UpdateListItemDto,
} from '../proxy/list-items';
import { RadioOptionDto, RadioOptionService } from '../proxy/radio-options';
import { FileAttachmentDto } from '../proxy/entity-attachments/file-attachments';
import { SHARED_IMPORTS } from '../shared/shared-imports.constants';
import { EntityAttachmentDto } from '../proxy/entity-attachments';
import { ConfirmationHelperService } from '../shared/services/confirmation-helper.service';
import { ToasterHelperService } from '../shared/services/toaster-helper.service';
import { ToasterService } from '@abp/ng.theme.shared';

type ListItemFormModel = {
  name: string;
  position: number;
  commentType: number | null;
  commentPlaceholder: string | null;
  isAttachmentRequired: boolean;
  isSeparator: boolean;
  concurrencyStamp: string | null;
};

@Component({
  standalone: true,
  selector: 'app-list-item',
  imports: [...SHARED_IMPORTS],
  templateUrl: './list-item.html',
  styleUrl: './list-item.scss',
  providers: [ListService],
})
export class ListItem implements OnInit {
  listItems = { items: [], totalCount: 0 } as PagedResultDto<ListItemDto>;

  tagInputVisible = false;
  tagInputValue = '';
  @ViewChild('tagInputEl', { static: false }) tagInputEl?: ElementRef<HTMLInputElement>;

  form!: FormGroup;
  isModalOpen = false;
  selected = {} as ListItemDto;
  filters = {} as GetListItemListDto;

  checkListId: string | null = null;
  modelCategoryId: string | null = null;
  checkListName: string | null = null;

  commentTypes = commentTypeOptions;

  radioOptions: RadioOptionDto[] = [];
  pendingRadioNames: string[] = [];
  isRadioBusy = false;

  tempFiles: FileAttachmentDto[] = []; // for file attachments
  existingFiles: EntityAttachmentDto[] = []; // for existing attachments

  public readonly list = inject(ListService);
  private readonly listItemService = inject(ListItemService);
  private readonly fb = inject(FormBuilder);
  private readonly confirmation = inject(ConfirmationHelperService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly  customToaster = inject(ToasterHelperService);
  private readonly checkListService = inject(CheckListService);
  private readonly radioOptionService = inject(RadioOptionService);
  private readonly toaster = inject(ToasterService)

  ngOnInit(): void {
    this.checkListId = this.route.snapshot.queryParamMap.get('checkListId');
    this.filters.checkListId = this.checkListId;
    this.modelCategoryId = this.route.snapshot.queryParamMap.get('modelCategoryId');
    this.loadCheckListName();

    this.buildForm();
    this.attachSeparatorBehavior();

    const streamCreator = query => {
      const input: GetListItemListDto = {
        skipCount: query.skipCount,
        maxResultCount: query.maxResultCount,
        sorting: query.sorting || 'position asc, name asc',
        checkListId: this.checkListId ?? undefined,
        filter: this.filters.filter,
      };

      return this.listItemService.getList(input);
    };

    this.list.hookToQuery(streamCreator).subscribe(res => (this.listItems = res));
    this.list.get();
  }

  createListItem(): void {
    this.resetAttachment();
    this.resetForm();
    this.clearRadioUi();
    this.isModalOpen = true;
  }

  closeModal(): void {
    this.isModalOpen = false;
    this.resetForm();
    this.clearRadioUi();
  }

  edit(id: string): void {
    this.listItemService.get(id).subscribe(dto => {
      this.selected = dto;

      this.form.setValue({
        name: dto.name ?? '',
        position: dto.position ?? 0,
        commentType: dto.commentType ?? null,
        commentPlaceholder: dto.commentPlaceholder ?? null,
        isAttachmentRequired: dto.isAttachmentRequired ?? false,
        isSeparator: dto.isSeparator ?? false,
        concurrencyStamp: dto.concurrencyStamp ?? null,
      });

      this.applySeparatorState(!!dto.isSeparator);
      this.getExisitingAttachments(dto);

      this.isModalOpen = true;

      this.pendingRadioNames = [];
      this.loadRadioOptions();
    });
  }

  save(): void {
    if (!this.checkListId) return;

    const raw = this.form.getRawValue() as ListItemFormModel;
    const isSep = !!raw.isSeparator;

    const name = (raw.name || '').trim();
    if (!name) return;

    if (!isSep && raw.commentType == null) {
      this.toaster.warn('::CommentTypeIsRequired');
      return;
    }

    const placeholderTrimmed = (raw.commentPlaceholder || '').trim();

    const commentType = isSep ? null : raw.commentType;
    const commentPlaceholder = isSep ? null : placeholderTrimmed || name;
    const isAttachmentRequired = isSep ? false : !!raw.isAttachmentRequired;

    if (this.selected?.id) {
      const input: UpdateListItemDto = {
        checkListId: this.checkListId,
        name,
        position: raw.position,
        commentType,
        commentPlaceholder,
        isAttachmentRequired,
        isSeparator: isSep,
        concurrencyStamp: raw.concurrencyStamp ?? this.selected.concurrencyStamp,
        tempFiles: this.tempFiles,
        entityAttachments: this.existingFiles,
      };

      this.listItemService.update(this.selected.id, input).subscribe(updated => {
        this.selected = updated ?? this.selected;

        this.resetAttachment();
        this.savePendingRadioOptions(this.selected.id);
        this.isModalOpen = false;
        this.list.get();
        this.loadRadioOptions();
      });

      return;
    }

    const input: CreateListItemDto = {
      checkListId: this.checkListId,
      name,
      position: raw.position,
      commentType,
      commentPlaceholder,
      isAttachmentRequired,
      isSeparator: isSep,
      tempFiles: this.tempFiles,
    };

    this.listItemService.create(input).subscribe(created => {
      this.customToaster.created();
      this.selected = created;

      this.resetAttachment();
      this.savePendingRadioOptions(created.id);

      this.list.get();
      this.loadRadioOptions();
    });
  }

  delete(id: string): void {
    this.confirmation.confirmDelete().subscribe(status => {
      if (status !== 'confirm') return;

      this.listItemService.delete(id).subscribe(() => this.list.get());
    });
    if (this.selected?.id === id) {
      this.closeModal();
    }
  }

  loadRadioOptions(): void {
    const listItemId = this.selected?.id;
    if (!listItemId) {
      this.radioOptions = [];
      return;
    }

    this.isRadioBusy = true;
    this.radioOptionService.getList({ listItemId }).subscribe({
      next: res => (this.radioOptions = res ?? []),
      error: () => (this.isRadioBusy = false),
      complete: () => (this.isRadioBusy = false),
    });
  }

  private savePendingRadioOptions(listItemId: string): void {
    const names = (this.pendingRadioNames || []).map(x => x.trim()).filter(Boolean);
    if (!names.length) return;

    this.isRadioBusy = true;

    const dto = { listItemId, names } as any;

    this.radioOptionService.create(dto).subscribe({
      next: (res: any) => {
        const created: RadioOptionDto[] = Array.isArray(res) ? res : res ? [res] : [];

        const existingById = new Map<string, RadioOptionDto>(this.radioOptions.map(x => [x.id, x]));
        for (const c of created) {
          if (c?.id) existingById.set(c.id, c);
        }
        this.radioOptions = Array.from(existingById.values());

        this.pendingRadioNames = [];
        this.customToaster.created();
      },
      error: () => (this.isRadioBusy = false),
      complete: () => (this.isRadioBusy = false),
    });
  }

  private buildForm(): void {
    this.form = this.fb.group({
      name: [this.selected.name ?? '', [Validators.required, Validators.maxLength(128)]],
      position: [this.selected.position ?? 0, [Validators.required, Validators.min(0)]],
      commentType: [this.selected.commentType ?? null],
      commentPlaceholder: [this.selected.commentPlaceholder ?? null, [Validators.maxLength(128)]],
      isAttachmentRequired: [this.selected.isAttachmentRequired ?? false],
      isSeparator: [this.selected.isSeparator ?? false],
      concurrencyStamp: [this.selected.concurrencyStamp ?? null],
    });

    this.applySeparatorState(!!this.form.get('isSeparator')?.value);
  }

  private attachSeparatorBehavior(): void {
    this.form.get('isSeparator')?.valueChanges.subscribe((isSep: boolean) => {
      this.applySeparatorState(!!isSep);
    });
  }

  private applySeparatorState(isSep: boolean): void {
    const attCtrl = this.form.get('isAttachmentRequired');
    const typeCtrl = this.form.get('commentType');
    const placeholderCtrl = this.form.get('commentPlaceholder');

    if (isSep) {
      attCtrl?.setValue(false, { emitEvent: false });
      typeCtrl?.setValue(null, { emitEvent: false });
      placeholderCtrl?.setValue(null, { emitEvent: false });

      attCtrl?.disable({ emitEvent: false });
      typeCtrl?.disable({ emitEvent: false });
      placeholderCtrl?.disable({ emitEvent: false });
    } else {
      attCtrl?.enable({ emitEvent: false });
      typeCtrl?.enable({ emitEvent: false });
      placeholderCtrl?.enable({ emitEvent: false });

      if (typeCtrl?.value == null) {
        typeCtrl.setValue(this.commentTypes?.[0]?.value ?? 1, { emitEvent: false });
      }

      if (placeholderCtrl?.value == null) {
        placeholderCtrl.setValue('', { emitEvent: false });
      }
    }
  }

  private resetForm(): void {
    this.resetAttachment();
    this.selected = {} as ListItemDto;

    this.form.reset({
      name: '',
      position: 0,
      commentType: this.commentTypes?.[0]?.value ?? 1,
      commentPlaceholder: '',
      isAttachmentRequired: false,
      isSeparator: false,
      concurrencyStamp: null,
    });

    this.applySeparatorState(false);
  }

  private loadCheckListName(): void {
    if (!this.checkListId) {
      this.checkListName = null;
      return;
    }

    this.checkListService.get(this.checkListId).subscribe(dto => {
      this.checkListName = dto?.name ?? null;
    });
  }

  private clearRadioUi(): void {
    this.radioOptions = [];
    this.pendingRadioNames = [];
    this.isRadioBusy = false;
  }

  goBack() {
    this.router.navigate(['/check-lists'], {
      queryParams: {
        carModelId: this.route.snapshot.queryParamMap.get('carModelId'),
        modelCategoryId: this.modelCategoryId,
      },
      queryParamsHandling: 'merge',
    });
  }

  resetAttachment() {
    this.tempFiles = [];
    this.existingFiles = [];
  }

  getExisitingAttachments(dto: ListItemDto): void {
    this.existingFiles = [...(dto.entityAttachments ?? [])];
  }

  private normalizeName(v: string): string {
    return (v || '').trim();
  }

  private normalizeKey(v: string): string {
    return this.normalizeName(v).toLowerCase();
  }

  showTagInput(): void {
    this.tagInputVisible = true;
    setTimeout(() => this.tagInputEl?.nativeElement?.focus(), 10);
  }

  confirmTagInput(): void {
    const name = this.normalizeName(this.tagInputValue);
    if (!name) {
      this.tagInputValue = '';
      this.tagInputVisible = false;
      return;
    }

    const key = this.normalizeKey(name);

    const existsInDb = (this.radioOptions ?? []).some(x => this.normalizeKey(x.name || '') === key);
    const existsPending = (this.pendingRadioNames ?? []).some(x => this.normalizeKey(x) === key);

    if (existsInDb || existsPending) {
      this.toaster.warn('::AlreadyExists');
    } else {
      this.pendingRadioNames = [...this.pendingRadioNames, name];
    }

    this.tagInputValue = '';
    this.tagInputVisible = false;
  }

  removePendingTag(name: string): void {
    const key = this.normalizeKey(name);
    this.pendingRadioNames = (this.pendingRadioNames ?? []).filter(
      x => this.normalizeKey(x) !== key
    );
  }

  removeDbTag(option: RadioOptionDto): void {
    if (!option?.id) return;

     this.confirmation.confirmDelete().subscribe(status => {
      if (status !== 'confirm') return;

      this.isRadioBusy = true;
      this.radioOptionService.delete(option.id).subscribe({
        next: () => {
          this.radioOptions = (this.radioOptions ?? []).filter(x => x.id !== option.id);
          this.customToaster.deleted();
        },
        error: () => (this.isRadioBusy = false),
        complete: () => (this.isRadioBusy = false),
      });
    });
  }
}
