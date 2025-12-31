import { PagedResultDto, ListService, LocalizationPipe } from '@abp/ng.core';
import {
  ConfirmationService,
  Confirmation,
  ThemeSharedModule,
  ToasterService,
} from '@abp/ng.theme.shared';
import { Component, inject, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PageModule } from '@abp/ng.components/page';
import { CommonModule } from '@angular/common';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { CarModelService, GetCarModelListDto } from '../proxy/car-models';
import { PermissionDirective } from '@abp/ng.core'; 
import {
  CheckListDto,
  CheckListService,
  UpdateCheckListDto,
  CreateCheckListDto,
  checkListTypeOptions,
  GetCheckListListDto,
} from '../proxy/check-lists';

@Component({
  standalone: true,
  selector: 'app-check-list',
  imports: [
    CommonModule,
    PageModule,
    ThemeSharedModule,
    LocalizationPipe,
    ReactiveFormsModule,
    NgbDropdownModule,
    PermissionDirective
  ],
  templateUrl: './check-list.html',
  styleUrl: './check-list.scss',
  providers: [ListService],
})
export class CheckList implements OnInit {
  checkLists = { items: [], totalCount: 0 } as PagedResultDto<CheckListDto>;

  form!: FormGroup;
  selected = {} as CheckListDto;

  carModelId: string | null = null;
  carModelName: string | null = null;

  filters = {} as GetCheckListListDto;

  checkListType = checkListTypeOptions;

  public readonly list = inject(ListService);
  private readonly service = inject(CheckListService);
  private readonly fb = inject(FormBuilder);
  private readonly confirmation = inject(ConfirmationService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly carModelService = inject(CarModelService);
  private readonly toaster = inject(ToasterService);

  ngOnInit(): void {
    debugger;
    this.buildForm();

    this.carModelId = this.route.snapshot.queryParamMap.get('carModelId');
    this.loadCarModelName();

    this.filters.carModelId = this.carModelId;

    const streamCreator = query => {
      const input: GetCheckListListDto = {
        skipCount: query.skipCount,
        maxResultCount: query.maxResultCount,
        sorting: query.sorting || 'position asc, name asc',
        carModelId: this.carModelId ?? undefined,
      };

      return this.service.getList(input);
    };

    this.list.hookToQuery(streamCreator).subscribe(res => (this.checkLists = res));
    this.list.get();
  }

  private loadCarModelName(): void {
    if (!this.carModelId) {
      this.carModelName = null;
      return;
    }

    const input: GetCarModelListDto = {
      skipCount: 0,
      maxResultCount: 1000,
      sorting: 'name asc',
    };

    this.carModelService.getList(input).subscribe(res => {
      const m = (res.items ?? []).find(x => x.id === this.carModelId);
      this.carModelName = m?.name ?? null;
    });
  }

  buildForm(): void {
    this.form = this.fb.group({
      name: [this.selected.name ?? '', [Validators.required, Validators.maxLength(128)]],
      position: [this.selected.position ?? 0, [Validators.required, Validators.min(0)]],
      checkListType: [this.selected.checkListType ?? 0, Validators.required],
      concurrencyStamp: [null],
    });
  }

  resetForm(): void {
    this.selected = {} as CheckListDto;
    this.form.reset({
      name: '',
      position: 0,
      checkListType: this.checkListType ?? 0,
      concurrencyStamp: null,
    });
  }

  edit(id: string): void {
    this.service.get(id).subscribe(dto => {
      this.selected = dto;

      this.form.setValue({
        name: dto.name ?? '',
        position: dto.position ?? 0,
        checkListType: dto.checkListType,
        concurrencyStamp: (dto as UpdateCheckListDto).concurrencyStamp ?? null,
      });
    });
  }

  save(): void {
    if (this.form.invalid || !this.carModelId) return;

    const raw = this.form.getRawValue();
    const name = (raw.name || '').trim();

    if (this.selected.id) {
      const input: UpdateCheckListDto = {
        carModelId: this.carModelId,
        name,
        position: raw.position,
        checkListType: raw.checkListType,
        concurrencyStamp: raw.concurrencyStamp,
      };

      this.service.update(this.selected.id, input).subscribe(() => {
        this.toaster.success('::SuccessfullyUpdated.');
        this.resetForm();
        this.list.get();
      });
      return;
    }

    const input: CreateCheckListDto = {
      carModelId: this.carModelId,
      name,
      position: raw.position,
      checkListType: raw.checkListType,
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
    this.router.navigate(['/car-models']);
  }

  addListItem(checkListId: string): void {
    this.router.navigate(['list-items'], { queryParams: { checkListId }});
  }
}
