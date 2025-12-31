import { PagedResultDto, ListService, LocalizationPipe, PermissionDirective } from '@abp/ng.core';
import { ConfirmationService, ToasterService, Confirmation, ThemeSharedModule } from '@abp/ng.theme.shared';
import { Component, inject, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CheckListService, UpdateCheckListDto } from '../proxy/check-lists';
import { ListItemDto, commentTypeOptions, ListItemService, GetListItemListDto, UpdateListItemDto, CreateListItemDto } from '../proxy/list-items';
import { PageModule } from '@abp/ng.components/page';
import { CommonModule } from '@angular/common';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';

@Component({
  standalone: true,
  selector: 'app-list-item',
  imports: [
    CommonModule,
    PageModule,
    ThemeSharedModule,
    LocalizationPipe,
    ReactiveFormsModule,
    NgbDropdownModule,
    PermissionDirective,
  ],
  templateUrl: './list-item.html',
  styleUrl: './list-item.scss',
  providers: [ListService]
})
export class ListItem implements OnInit{
 listItems = { items: [], totalCount: 0 } as PagedResultDto<ListItemDto>;

  form!: FormGroup;
  selected = {} as ListItemDto;

  checkListId: string | null = null;
  checkListName: string | null = null;

  commentTypes = commentTypeOptions;

  public readonly list = inject(ListService);
  private readonly service = inject(ListItemService);
  private readonly fb = inject(FormBuilder);
  private readonly confirmation = inject(ConfirmationService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly toaster = inject(ToasterService);

  private readonly checkListService = inject(CheckListService);

  ngOnInit(): void {
    this.buildForm();

    this.checkListId = this.route.snapshot.queryParamMap.get('checkListId');
    this.loadCheckListName();

    const streamCreator = (query) => {
      const input: GetListItemListDto = {
        skipCount: query.skipCount,
        maxResultCount: query.maxResultCount,
        sorting: query.sorting || 'position asc, name asc',
        checkListId: this.checkListId ?? undefined,
      };

      return this.service.getList(input);
    };

    this.list.hookToQuery(streamCreator).subscribe(res => (this.listItems = res));
    this.list.get();

    this.form.get('isSeparator')?.valueChanges.subscribe((isSep: boolean) => {
      const att = this.form.get('isAttachmentRequired');
      if (!att) return;

      if (isSep) {
        att.setValue(false, { emitEvent: false });
        att.disable({ emitEvent: false });
      } else {
        att.enable({ emitEvent: false });
      }
    });
  }

  private loadCheckListName(): void {
    if (!this.checkListId) {
      this.checkListName = null;
      return;
    }

    this.checkListService.get(this.checkListId).subscribe((dto) => this.checkListName = dto?.name ?? null);
  }

  buildForm(): void {
    this.form = this.fb.group({
      name: [this.selected.name ?? '', [Validators.required, Validators.maxLength(256)]],
      position: [this.selected.position ?? 0, [Validators.required, Validators.min(0)]],
      commentType: [this.selected.commentType ?? this.commentTypes?.[0]?.value ?? 1, Validators.required],
      commentPlaceholder: [this.selected.commentPlaceholder ?? '', [Validators.maxLength(256)]],
      isAttachmentRequired: [this.selected.isAttachmentRequired ?? false],
      isSeparator: [this.selected.isSeparator ?? false],
      concurrencyStamp: [null],
    });
  }

  resetForm(): void {
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

    this.form.get('isAttachmentRequired')?.enable({ emitEvent: false });
  }

  edit(id: string): void {
    this.service.get(id).subscribe(dto => {
      this.selected = dto;

      this.form.setValue({
        name: dto.name ?? '',
        position: dto.position ?? 0,
        commentType: dto.commentType,
        commentPlaceholder: dto.commentPlaceholder ?? '',
        isAttachmentRequired: dto.isAttachmentRequired ?? false,
        isSeparator: dto.isSeparator ?? false,
        concurrencyStamp: (dto as UpdateListItemDto).concurrencyStamp ?? null,
      });

      if (dto.isSeparator) {
        this.form.get('isAttachmentRequired')?.disable({ emitEvent: false });
      } else {
        this.form.get('isAttachmentRequired')?.enable({ emitEvent: false });
      }
    });
  }

  save(): void {
    if (this.form.invalid || !this.checkListId) return;

    const raw = this.form.getRawValue();

    const name = (raw.name || '').trim();
    const commentPlaceholder = (raw.commentPlaceholder || '').trim();

    if (this.selected.id) {
      const input: UpdateListItemDto = {
        checkListId: this.checkListId,
        name,
        position: raw.position,
        commentType: raw.commentType,
        commentPlaceholder: commentPlaceholder || name,
        isAttachmentRequired: raw.isSeparator ? false : raw.isAttachmentRequired,
        isSeparator: raw.isSeparator,
        concurrencyStamp: raw.concurrencyStamp,
      };

      this.service.update(this.selected.id, input).subscribe(() => {
        this.toaster.success('::SuccessfullyUpdated.');
        this.resetForm();
        this.list.get();
      });
      return;
    }

    const input: CreateListItemDto = {
      checkListId: this.checkListId,
      name,
      position: raw.position,
      commentType: raw.commentType,
      commentPlaceholder: commentPlaceholder || name,
      isAttachmentRequired: raw.isSeparator ? false : raw.isAttachmentRequired,
      isSeparator: raw.isSeparator,
    };

    this.service.create(input).subscribe(() => {
      this.toaster.success('::SuccessfullyCreated.');
      this.resetForm();
      this.list.get();
    });
  }

  delete(id: string): void {
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe(status => {
      if (status === Confirmation.Status.confirm) {
        this.service.delete(id).subscribe(() => {
          this.toaster.success('::SuccessfullyDeleted.');
          this.list.get();
        });
      }
    });
  }

  get isEditMode(): boolean {
    return !!this.selected?.id;
  }

  goBack(): void {
    this.router.navigate(['/check-lists'], {
      queryParams: {
        carModelId: this.route.snapshot.queryParamMap.get('carModelId'),
      },
      queryParamsHandling: 'merge',
    });
  }
}
