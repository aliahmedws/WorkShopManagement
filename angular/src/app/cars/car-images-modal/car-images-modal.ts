import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { CarDto, CarService } from 'src/app/proxy/cars';
import { StageDto } from 'src/app/proxy/stages';
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';

@Component({
  selector: 'app-car-images-modal',
  imports: [...SHARED_IMPORTS],
  templateUrl: './car-images-modal.html',
  styleUrl: './car-images-modal.scss'
})
export class CarImagesModal {

  private readonly carService = inject(CarService);
  private readonly toaster = inject(ToasterHelperService);

  @Input() visible = false;
  @Output() visibleChange = new EventEmitter<boolean>();
  @Input() carId = '';

  @Output() submit = new EventEmitter<void>();

  selectedCar = {} as CarDto;

  selectedImage: string | null = null;
  images: string[] = [];
  loading = false;
  saving = false;
  previewLoading = false;

  modalOptions = {
    size: 'xl',
    backdrop: 'static', //prevent close by outside click
    keyboard: false, //prevent close by esc key
    animation: true,
  };

  init(): void {
    this.carService.get(this.carId).subscribe((car) => {
      this.selectedCar = car;
    });
  
    this.images = [];
    this.selectedImage = this.selectedCar.imageLink || null;

    // Only fetch automatically if there is no image saved yet
    if (!this.selectedImage) {
      this.getCarImages();
    }
  }

  // Add Selection Logic
  selectImage(url: string): void {

    // if(this.selectedImage === url) return;

    this.previewLoading = true;
    this.selectedImage = url;
    this.previewLoading = false;
  }



  getCarImages(): void {
    this.images = [];
    this.loading = true;
    this.carService.getExternalCarImages(this.carId!).subscribe((res) => {
      this.images = res;
      this.loading = false;
    });
  }

  // Add Save Logic
save(): void {
  if (!this.selectedImage) return;

  this.saving = true;
  this.carService.saveCarImage(this.carId, this.selectedImage).subscribe(() => {
      this.toaster.createdOrUpdated('::CarImageSaved');
      this.saving = false;

      this.submit.emit();
      this.close();
  });
}

  close(): void {
    this.visible = false;
    this.visibleChange.emit(this.visible);
  }


    // dont need this can directyly do that
  onPreviewLoad(): void {
    this.previewLoading = false;
  }

}

