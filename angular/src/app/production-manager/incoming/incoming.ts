import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, EventEmitter, inject, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { CheckInReportModal } from 'src/app/check-in-reports/check-in-report-modal/check-in-report-modal';
import { CarService, CarDto, GetCarListInput } from 'src/app/proxy/cars';
import { Recalls } from 'src/app/recalls/recalls';
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { CarNotesModal } from "src/app/cars/car-notes-modal/car-notes-modal";

@Component({
  selector: 'app-incoming',
  imports: [...SHARED_IMPORTS, Recalls, CheckInReportModal, CarNotesModal],
  templateUrl: './incoming.html',
  styleUrl: './incoming.scss'
})
export class Incoming {
  @Input() filters: any = {};
  @Output() filtersChange = new EventEmitter<any>();
  @Input() list: ListService;
  @Input() cars: PagedResultDto<CarDto> = { items: [], totalCount: 0 };

  private readonly fb = inject(FormBuilder);
  private readonly carService = inject(CarService);
  private readonly toaster = inject(ToasterHelperService);

  notesForm!: FormGroup;

  @ViewChild('notesModal', { static: true })
  notesModal!: CarNotesModal;

  selectedCar = {} as CarDto;
  isRecallModalVisible = false;
  isCheckInModalVisible = false;

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

  onNotesSaved(e: { carId: string; notes: string }): void {
    const row = this.cars.items?.find(x => x.id === e.carId);
    if (row) {
      row.notes = e.notes;
    }
    this.list.get();
  }

  
}