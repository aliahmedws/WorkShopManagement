import { PagedResultDto, ListService, LocalizationPipe } from '@abp/ng.core';
import { ConfirmationService, ToasterService, Confirmation, ThemeSharedModule } from '@abp/ng.theme.shared';
import { Component, inject, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CarModelDto, CarModelService, GetCarModelListDto, UpdateCarModelDto } from '../proxy/car-models';
import { CustomCarModelUploadService } from 'src/custom-services/custom-car-model-service';
import { PageModule } from '@abp/ng.components/page';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-car-model',
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    PageModule,
    ThemeSharedModule,
    LocalizationPipe
  ],
  templateUrl: './car-model.html',
  styleUrl: './car-model.scss',
  providers: [ListService],
})
export class CarModel implements OnInit {
 carModels = { items: [], totalCount: 0 } as PagedResultDto<CarModelDto>;

  isModalOpen = false;
  modalBusy = false;

  form: FormGroup;
  selectedCarModel = {} as CarModelDto;

  selectedFile?: File;
  fileError?: string;

  filters = {} as GetCarModelListDto;

  public readonly list = inject(ListService);
  private readonly carModelService = inject(CarModelService);
  private readonly uploadService = inject(CustomCarModelUploadService);
  private readonly fb = inject(FormBuilder);
  private readonly confirmation = inject(ConfirmationService);
  private readonly toaster = inject(ToasterService);

  ngOnInit(): void {
    const streamCreator = (query) => this.carModelService.getList({...query, ...this.filters});

    this.list.hookToQuery(streamCreator).subscribe((response) => {
      this.carModels = response;
    });
  }

  search(): void {
    this.list.get();
  }

  clearSearch(): void {
    this.filters = {} as GetCarModelListDto;
    this.list.get();
  }

  createCarModel(): void {
    this.selectedCarModel = {} as CarModelDto;
    this.selectedFile = undefined;
    this.fileError = undefined;
    this.buildForm(false);
    this.isModalOpen = true;
  }

  editCarModel(id: string): void {
    this.carModelService.get(id).subscribe((carModel) => {
      this.selectedCarModel = carModel;
      this.selectedFile = undefined;
      this.fileError = undefined;
      this.buildForm(true);
      this.isModalOpen = true;
    });
  }

  onFileSelected(ev: Event): void {
    const input = ev.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    this.selectedFile = file;
    this.fileError = undefined;
  }

  private buildForm(isEdit: boolean): void {
    this.form = this.fb.group({
      name: [
        this.selectedCarModel.name || '',
        [Validators.required, Validators.maxLength(128)],
      ],
      description: [
        this.selectedCarModel.description ?? null,
        [Validators.maxLength(512)],
      ],
      concurrencyStamp: [
        (this.selectedCarModel as any).concurrencyStamp ?? null,
        isEdit ? [Validators.required] : [],
      ],
    });
  }

  save(): void {
    if (this.form.invalid) {
      return;
    }

    this.modalBusy = true;

    if (this.selectedCarModel.id) {
      const payload: UpdateCarModelDto = {
        name: this.form.value.name,
        description: this.form.value.description,
        concurrencyStamp: this.form.value.concurrencyStamp,
      };

      this.carModelService.update(this.selectedCarModel.id, payload).subscribe({
        next: () => {
          this.toaster.success('Car model updated.');
          this.isModalOpen = false;
          this.form.reset();
          this.list.get();
        },
        error: () => {
          this.toaster.error(
            'Update failed. The record may have been modified by another user.'
          );
        },
        complete: () => (this.modalBusy = false),
      });

      return;
    }

    if (!this.selectedFile) {
      this.fileError = 'File is required.';
      this.modalBusy = false;
      return;
    }

    const createInput: any = {
      name: this.form.value.name,
      description: this.form.value.description,
    };

    const fd = new FormData();
    fd.append('file', this.selectedFile, this.selectedFile.name);
    fd.append('Name', createInput.name);
    if (createInput.description) fd.append('Description', createInput.description);

    this.uploadService.uploadFormData(fd).subscribe({
      next: () => {
        this.toaster.success('Car model created.');
        this.isModalOpen = false;
        this.form.reset();
        this.selectedFile = undefined;
        this.list.get();
      },
      error: () => {
        this.toaster.error('Upload failed.');
      },
      complete: () => (this.modalBusy = false),
    });
  }

  delete(id: string): void {
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.carModelService.delete(id).subscribe(() => {
          this.toaster.success('Car model deleted.');
          this.list.get();
        });
      }
    });
  }

  get isEditMode(): boolean {
    return !!this.selectedCarModel?.id;
  }
}


