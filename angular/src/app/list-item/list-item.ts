// list-item.ts
import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import { LocalizationPipe, ListService, PagedResultDto, PermissionDirective } from '@abp/ng.core';
import {
  Confirmation,
  ConfirmationService,
  ThemeSharedModule,
  ToasterService,
} from '@abp/ng.theme.shared';
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
import { TempFileDto, UploadFileService } from '../entity-attachment/upload-files.service';

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
  pendingRadioNames: string[] = [];
  radioOptionName = '';
  isRadioBusy = false;

  selectedFiles: { file: File; url: string | ArrayBuffer | null; name: string }[] = [];
  uploadedFiles: TempFileDto[] = [];

  public readonly list = inject(ListService);
  private readonly listItemService = inject(ListItemService);
  private readonly fb = inject(FormBuilder);
  private readonly confirmation = inject(ConfirmationService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly toaster = inject(ToasterService);
  private readonly checkListService = inject(CheckListService);
  private readonly radioOptionService = inject(RadioOptionService);
  private readonly uploadService = inject(UploadFileService);

  ngOnInit(): void {
    this.checkListId = this.route.snapshot.queryParamMap.get('checkListId');
    this.loadCheckListName();

    this.buildForm();
    this.attachSeparatorBehavior();

    const streamCreator = query => {
      const input: GetListItemListDto = {
        skipCount: query.skipCount,
        maxResultCount: query.maxResultCount,
        sorting: query.sorting || 'position asc, name asc',
        checkListId: this.checkListId ?? undefined,
      };

      return this.listItemService.getList(input);
    };

    this.list.hookToQuery(streamCreator).subscribe(res => (this.listItems = res));
    this.list.get();
  }

  createListItem(): void {
    this.resetAttachmentAfterSave();
    this.resetForm();
    this.clearRadioUi();
    this.isModalOpen = true;
  }

  closeModal(): void {
    this.isModalOpen = false;
    this.resetAttachmentAfterSave();
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
      this.uploadedFiles = [];
      this.selectedFiles = [];

      this.isModalOpen = true;

      this.pendingRadioNames = [];
      this.radioOptionName = '';
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
        tempFiles: this.uploadedFiles,
        attachments: this.selected.attachments,
      };

      this.listItemService.update(this.selected.id, input).subscribe(updated => {
        this.selected = updated ?? this.selected;

        this.resetAttachmentAfterSave();
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
      tempFiles: this.uploadedFiles,
    };

    this.listItemService.create(input).subscribe(created => {
      this.toaster.success('::SuccessfullyCreated.');
      this.selected = created;

      this.resetAttachmentAfterSave();
      this.savePendingRadioOptions(created.id);

      this.list.get();
      this.loadRadioOptions();
    });
  }

  delete(id: string): void {
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe(status => {
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

  onFileSelected(event: any): void {
    const files: FileList = event.target.files;
    if (files && files.length > 0) {
      const formData = new FormData();
      for (let i = 0; i < files.length; i++) {
        formData.append('files', files[i]);
      }

      this.uploadService.uploadFile(formData).subscribe({
        next: (res: TempFileDto[]) => {
          this.uploadedFiles = [...this.uploadedFiles, ...(res ?? [])];
          event.target.value = '';
        },
        error: err => {
          this.toaster.error('::UploadFailed');
          console.error(err);
        },
      });
    }
  }

  removeTempFile(index: number): void {
    this.uploadedFiles.splice(index, 1);
  }

  removeExistingAttachment(index: number): void {
    const existing = this.selected?.attachments ?? [];
    existing.splice(index, 1);
    this.selected.attachments = [...existing];
  }

  isImage(name: string): boolean {
    if (!name) return false;
    return /\.(jpg|jpeg|png|gif|webp|bmp)$/i.test(name);
  }

  private clearAttachmentUi(): void {
    this.selectedFiles = [];
    this.uploadedFiles = [];
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

  addRadioOption(): void {
    const name = (this.radioOptionName || '').trim();
    if (!name) return;

    const lower = name.toLowerCase();

    const existsInDb = this.radioOptions.some(x => (x.name || '').trim().toLowerCase() === lower);
    const existsPending = this.pendingRadioNames.some(x => x.toLowerCase() === lower);

    if (existsInDb || existsPending) {
      this.toaster.warn('::AlreadyExists');
      return;
    }

    this.pendingRadioNames = [...this.pendingRadioNames, name];
    this.radioOptionName = '';
  }

  removePendingRadio(name: string): void {
    this.pendingRadioNames = this.pendingRadioNames.filter(x => x !== name);
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
        this.toaster.success('::SuccessfullyCreated.');
      },
      error: () => (this.isRadioBusy = false),
      complete: () => (this.isRadioBusy = false),
    });
  }

  deleteRadioOption(id: string): void {
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe(status => {
      if (status !== Confirmation.Status.confirm) return;

      this.isRadioBusy = true;
      this.radioOptionService.delete(id).subscribe({
        next: () => {
          this.radioOptions = this.radioOptions.filter(x => x.id !== id);
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
    this.radioOptionName = '';
    this.isRadioBusy = false;
  }

  resetAttachmentAfterSave() {
    this.uploadedFiles = [];
    this.selectedFiles = [];
  }

  goBack() {
    this.router.navigate(['/check-lists'], {
      queryParams: { carModelId: this.route.snapshot.queryParamMap.get('carModelId') },
      queryParamsHandling: 'merge',
    });
  }
}
