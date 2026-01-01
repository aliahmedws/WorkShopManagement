import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import { LocalizationPipe, ListService, PagedResultDto, PermissionDirective } from '@abp/ng.core';
import { Confirmation, ConfirmationService, ThemeSharedModule, ToasterService } from '@abp/ng.theme.shared';
import { PageModule } from '@abp/ng.components/page';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';

import { CheckListService } from '../proxy/check-lists';
import {
  commentTypeOptions,
  CreateListItemDto,
  GetListItemListDto,
  ListItemDto,
  ListItemService,
  UpdateListItemDto,
} from '../proxy/list-items';
import { CreateRadioOptionDto, RadioOptionDto, RadioOptionService } from '../proxy/radio-options';

@Component({
  standalone: true,
  selector: 'app-list-item',
  imports: [
    CommonModule,
    PageModule,
    ThemeSharedModule,
    FormsModule,
    ReactiveFormsModule,
    NgbDropdownModule,
    PermissionDirective,
    LocalizationPipe,
  ],
  templateUrl: './list-item.html',
  styleUrl: './list-item.scss',
  providers: [ListService],
})
export class ListItem implements OnInit {
  listItems = { items: [], totalCount: 0 } as PagedResultDto<ListItemDto>;

  form!: FormGroup;
  isModalOpen = false;
  selected = {} as ListItemDto;

  checkListId: string | null = null;
  checkListName: string | null = null;

  commentTypes = commentTypeOptions;

  radioOptions: RadioOptionDto[] = [];
  radioOptionName = '';
  isRadioBusy = false;

  public readonly list = inject(ListService);
  private readonly listItemService = inject(ListItemService);
  private readonly fb = inject(FormBuilder);
  private readonly confirmation = inject(ConfirmationService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly toaster = inject(ToasterService);
  private readonly checkListService = inject(CheckListService);
  private readonly radioOptionService = inject(RadioOptionService);

  ngOnInit(): void {
    this.checkListId = this.route.snapshot.queryParamMap.get('checkListId');
    this.loadCheckListName();

    this.buildForm();
    this.attachSeparatorBehavior();

    const streamCreator = (query) => {
      const input: GetListItemListDto = {
        skipCount: query.skipCount,
        maxResultCount: query.maxResultCount,
        sorting: query.sorting || 'position asc, name asc',
        checkListId: this.checkListId ?? undefined,
      };

      return this.listItemService.getList(input);
    };

    this.list.hookToQuery(streamCreator).subscribe((res) => (this.listItems = res));
    this.list.get();
  }

  createListItem(): void {
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
    this.listItemService.get(id).subscribe((dto) => {
      this.selected = dto;

      this.form.setValue({
        name: dto.name ?? '',
        position: dto.position ?? 0,
        commentType: dto.commentType ?? null,
        commentPlaceholder: dto.commentPlaceholder ?? null,
        isAttachmentRequired: dto.isAttachmentRequired ?? false,
        isSeparator: dto.isSeparator ?? false,
        concurrencyStamp: (dto as UpdateListItemDto).concurrencyStamp ?? null,
      });

      this.applySeparatorState(!!dto.isSeparator);

      this.isModalOpen = true;

      this.loadRadioOptions();
    });
  }

  save(){
    if (!this.checkListId) return;

    const raw = this.form.getRawValue();
    const isSep = !!raw.isSeparator;

    const name = (raw.name || '').trim();
    if (!name) return;

    if (!isSep && raw.commentType == null) {
      this.toaster.warn('::CommentTypeIsRequired');
      return;
    }

    const placeholderTrimmed = (raw.commentPlaceholder || '').trim();

    const commentType = isSep ? null : raw.commentType;
    const commentPlaceholder = isSep ? null : (placeholderTrimmed || name);
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
        concurrencyStamp: raw.concurrencyStamp,
      };

      this.listItemService.update(this.selected.id, input).subscribe((updated) => {
        this.toaster.success('::SuccessfullyUpdated.');

        this.selected = updated ?? this.selected;

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
    };

    this.listItemService.create(input).subscribe((created) => {
      this.toaster.success('::SuccessfullyCreated.');

      this.selected = created;

      this.list.get();
      this.loadRadioOptions();
    });
  }

  delete(id: string): void {
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
      if (status !== Confirmation.Status.confirm) return;

      this.listItemService.delete(id).subscribe(() => {
        this.toaster.success('::SuccessfullyDeleted.');
        this.list.get();

        if (this.selected?.id === id) {
          this.closeModal();
        }
      });
    });
  }

  // Radio options

  loadRadioOptions(): void {
    const listItemId = this.selected?.id;
    if (!listItemId) {
      this.radioOptions = [];
      return;
    }

    this.isRadioBusy = true;
    this.radioOptionService.getList({ listItemId }).subscribe({
      next: (res) => (this.radioOptions = res ?? []),
      error: () => (this.isRadioBusy = false),
      complete: () => (this.isRadioBusy = false),
    });
  }

  addRadioOption(): void {
    const listItemId = this.selected?.id;
    const name = (this.radioOptionName || '').trim();

    if (!listItemId) {
      this.toaster.warn('::SaveListItemFirst');
      return;
    }

    if (!name) return;

    const dto: CreateRadioOptionDto = { listItemId, name };

    this.isRadioBusy = true;
    this.radioOptionService.create(dto).subscribe({
      next: (created) => {
        this.radioOptions = [...this.radioOptions.filter((x) => x.id !== created.id), created];
        this.radioOptionName = '';
        this.toaster.success('::SuccessfullyCreated.');
      },
      error: () => (this.isRadioBusy = false),
      complete: () => (this.isRadioBusy = false),
    });
  }

  deleteRadioOption(id: string): void {
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
      if (status !== Confirmation.Status.confirm) return;

      this.isRadioBusy = true;
      this.radioOptionService.delete(id).subscribe({
        next: () => {
          this.radioOptions = this.radioOptions.filter((x) => x.id !== id);
          this.toaster.success('::SuccessfullyDeleted.');
        },
        error: () => (this.isRadioBusy = false),
        complete: () => (this.isRadioBusy = false),
      });
    });
  }

  // Separator behavior
  private buildForm(): void {
    this.form = this.fb.group({
      name: [this.selected.name ?? '', [Validators.required, Validators.maxLength(256)]],
      position: [this.selected.position ?? 0, [Validators.required, Validators.min(0)]],
      commentType: [this.selected.commentType ?? null],
      commentPlaceholder: [this.selected.commentPlaceholder ?? null, [Validators.maxLength(256)]],
      isAttachmentRequired: [this.selected.isAttachmentRequired ?? false],
      isSeparator: [this.selected.isSeparator ?? false],
      concurrencyStamp: [null],
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
      // FORCE values
      attCtrl?.setValue(false, { emitEvent: false });
      typeCtrl?.setValue(null, { emitEvent: false });
      placeholderCtrl?.setValue(null, { emitEvent: false });

      // DISABLE controls
      attCtrl?.disable({ emitEvent: false });
      typeCtrl?.disable({ emitEvent: false });
      placeholderCtrl?.disable({ emitEvent: false });
    } else {
      // ENABLE controls
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

    this.checkListService.get(this.checkListId).subscribe((dto) => {
      this.checkListName = dto?.name ?? null;
    });
  }

  private clearRadioUi(): void {
    this.radioOptions = [];
    this.radioOptionName = '';
    this.isRadioBusy = false;
  }

  goBack(): void {
    this.router.navigate(['/check-lists'], {
      queryParams: { carModelId: this.route.snapshot.queryParamMap.get('carModelId') },
      queryParamsHandling: 'merge',
    });
  }
}
