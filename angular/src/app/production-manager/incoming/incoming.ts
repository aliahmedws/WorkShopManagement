import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, EventEmitter, inject, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { CheckInReportModal } from 'src/app/check-in-reports/check-in-report-modal/check-in-report-modal';
import { CarService, CarDto, GetCarListInput } from 'src/app/proxy/cars';
import { Recalls } from 'src/app/recalls/recalls';
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { CarNotesModal } from "src/app/cars/car-notes-modal/car-notes-modal";
import { CarImagesModal } from 'src/app/cars/car-images-modal/car-images-modal';
import { CarCreateEditModal } from 'src/app/cars/car-create-edit-modal/car-create-edit-modal';
import { IssueModal } from 'src/app/issues/issue-modal/issue-modal';
import { ConfirmationHelperService } from 'src/app/shared/services/confirmation-helper.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-incoming',
  imports: [...SHARED_IMPORTS, Recalls,
    CheckInReportModal,
    CarNotesModal,
    CarImagesModal,
    CarCreateEditModal,
    IssueModal,],
  templateUrl: './incoming.html',
  styleUrl: './incoming.scss'
})
export class Incoming {
  @Input() filters: any = {};
  @Output() filtersChange = new EventEmitter<any>();
  @Input() list: ListService;
  @Input() cars: PagedResultDto<CarDto> = { items: [], totalCount: 0 };

  private readonly carService = inject(CarService);
  private readonly confirmation = inject(ConfirmationHelperService);
  private readonly router = inject(Router);

  @ViewChild('notesModal', { static: true })
  notesModal!: CarNotesModal;

  selectedCar = {} as CarDto;
  isRecallModalVisible = false;
  isCheckInModalVisible = false;

  isEditModalVisible = false;
  selectedCarId?: string;

  isIssueModalVisible = false;
  isImageModalVisible = false;

  openRecallModal(car: CarDto): void {
    this.selectedCar = car;
    this.isRecallModalVisible = true;
  }

  openCheckInModal(car: CarDto  ): void {
    this.selectedCar = car;
    this.isCheckInModalVisible = true;
  }

  openNotesModal(car: CarDto): void {
    this.selectedCar = car;

    if (!car.id) return;
    this.notesModal.open(car.id, car.notes);
  }

   edit(carId: string): void {
    this.selectedCarId = carId;
    this.isEditModalVisible = true;
  }

  delete(carId: string): void {
    this.confirmation.confirmDelete().subscribe(status => {
      if (status !== 'confirm') return;

      this.carService.delete(carId).subscribe(() => this.list.get());
    });
  }

  showIssues(car: CarDto): void {
    this.selectedCarId = car?.id;
    this.isIssueModalVisible = true;
  }

  showImages(car: CarDto): void {
    this.selectedCar = car;
    this.isImageModalVisible = true;
  }

  manageLogistics(carId: string, vin: string): void {
    this.router.navigate(['/logistics-details'], {
      queryParams: { carId, vin },
    });
  }

  onNotesSaved(e: { carId: string; notes: string }): void {
    const row = this.cars.items?.find(x => x.id === e.carId);
    if (row) {
      row.notes = e.notes;
    }
    this.list.get();
  }

  
}