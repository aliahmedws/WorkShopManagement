import { PagedResultDto, ListService, LocalizationPipe } from '@abp/ng.core';
import {
  ConfirmationService,
  Confirmation,
  ToasterService,
} from '@abp/ng.theme.shared';
import { Component, inject, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CarModelService, GetCarModelListDto } from '../proxy/car-models';
import { CheckListDto, GetCheckListListDto, CheckListService, UpdateCheckListDto, CreateCheckListDto } from '../proxy/check-lists';
import { SHARED_IMPORTS } from '../shared/shared-imports.constants';
import { FileAttachmentDto } from '../proxy/entity-attachments/file-attachments';
import { EntityAttachmentDto } from '../proxy/entity-attachments';
@Component({
  standalone: true,
  selector: 'app-check-list',
  imports: [...SHARED_IMPORTS],
  templateUrl: './check-list.html',
  styleUrl: './check-list.scss',
  providers: [ListService],
})
export class CheckList implements OnInit {
  checkLists = { items: [], totalCount: 0 } as PagedResultDto<CheckListDto>;
  form!: FormGroup;
  selected = {} as CheckListDto;
  carModelId: string | null = null;
  modelCategoryId:string | null = null;
  carModelName: string | null = null;
  isModalOpen = false;
  filters = {} as GetCheckListListDto;

  tempFiles: FileAttachmentDto[] = [];  // for file attachments
  existingFiles: EntityAttachmentDto[] = []; // for existing attachments
  
  public readonly list = inject(ListService);
  private readonly service = inject(CheckListService);
  private readonly fb = inject(FormBuilder);
  private readonly confirmation = inject(ConfirmationService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly carModelService = inject(CarModelService);
  private readonly toaster = inject(ToasterService);

  ngOnInit(): void {
    this.buildForm();
    this.carModelId = this.route.snapshot.queryParamMap.get('carModelId');
    this.modelCategoryId = this.route.snapshot.queryParamMap.get('modelCategoryId');
    this.loadCarModelName();
    this.filters.carModelId = this.carModelId;
    const streamCreator = (query: GetCheckListListDto) => this.service.getList({ ...query, ...this.filters });
    this.list
      .hookToQuery(streamCreator)
      .subscribe((response: PagedResultDto<CheckListDto>) => {
        this.checkLists = response;
      })
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
    const { name, position, concurrencyStamp } = this.selected || {};
    this.form = this.fb.group({
      name: [name ?? '', [Validators.required, Validators.maxLength(128)]],
      position: [position ?? 0, [Validators.required, Validators.min(0)]],
      concurrencyStamp: [concurrencyStamp ?? null],
    });
  }
  createCheckList() {
    if (!this.carModelId) return;
    this.resetAttachment();               // reset attachments on create - remove this line if persisting temp attachments on modal reopens
    this.selected = {} as CheckListDto;
    this.buildForm();
    this.isModalOpen = true;
  }
  editCheckList(id: string) {
    this.resetAttachment();               // reset attachments on edit
    this.service.get(id).subscribe(dto => {
      this.selected = dto;
      this.getExisitingAttachments(dto);
      this.buildForm();
      this.isModalOpen = true;
    })
  }
  resetForm(): void {
    this.resetAttachment();             // reset attachments on form reset
    this.selected = {} as CheckListDto;
    this.form.reset({
      name: '',
      position: 0,
      concurrencyStamp: null,
    });
  }
  edit(id: string): void {
    this.service.get(id).subscribe(dto => {
      this.selected = dto;
      this.form.setValue({
        name: dto.name ?? '',
        position: dto.position ?? 0,
        concurrencyStamp: dto.concurrencyStamp ?? null,
      });
    });
  }
  save(): void {
    if (this.form.invalid || !this.carModelId) return;
    debugger;
    const raw = this.form.getRawValue();
    const name = (raw.name || '').trim();
    if (this.selected.id) {

      const input: UpdateCheckListDto = {
        carModelId: this.carModelId,
        name,
        position: raw.position,
        concurrencyStamp: raw.concurrencyStamp,
        tempFiles: this.tempFiles,
        entityAttachments: this.existingFiles,
      };
      
      this.service.update(this.selected.id, input).subscribe(() => {
        this.resetForm();
        this.list.get();
        this.isModalOpen = false;
        this.toaster.success('::SuccessfullyUpdated.');
      });
      return;
    }
    const input: CreateCheckListDto = {
      carModelId: this.carModelId,
      name,
      position: raw.position,
      tempFiles: this.tempFiles,
    };
    this.service.create(input).subscribe(() => {
      this.resetForm();
      this.list.get();
      this.isModalOpen = false;
      this.toaster.success('::SuccessfullyCreated.');
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
  closeModal() {
    this.isModalOpen = false;
    this.resetForm();
  }
  get isEditMode(): boolean {
    return !!this.selected?.id;
  }

  goBack(): void {
  this.router.navigate(['/car-models'], {
    queryParams: { modelCategoryId: this.modelCategoryId }
  });
}

  addListItem(checkListId: string): void {
    this.router.navigate(['list-items'], { queryParams: { checkListId, carModelId: this.carModelId , modelCategoryId: this.modelCategoryId } });
  }

  resetAttachment() {
    this.tempFiles = [];
    this.existingFiles = [];
  }

  getExisitingAttachments(dto:CheckListDto): void {
    this.existingFiles = [...(dto.entityAttachments ?? [])];
  }

}