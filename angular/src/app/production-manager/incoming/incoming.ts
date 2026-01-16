import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, EventEmitter, inject, Input, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { CheckInReportModal } from 'src/app/check-in-reports/check-in-report-modal/check-in-report-modal';
import { CarService, CarDto } from 'src/app/proxy/cars';
import { Recalls } from 'src/app/recalls/recalls';
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { ProductionActions } from "../production-actions/production-actions";
import { NgbDateAdapter, NgbDateNativeAdapter } from '@ng-bootstrap/ng-bootstrap'; 
import { EstReleaseModal } from 'src/app/cars/est-release-modal/est-release-modal';
import { CarNotesModal } from 'src/app/cars/car-notes-modal/car-notes-modal';

@Component({
  selector: 'app-incoming',
  imports: [...SHARED_IMPORTS, Recalls, CheckInReportModal, ProductionActions, EstReleaseModal, CarNotesModal],
  templateUrl: './incoming.html',
  styleUrl: './incoming.scss',
   providers: [{ provide: NgbDateAdapter, useClass: NgbDateNativeAdapter }],
})
export class Incoming {
  @Input() filters: any = {};
  @Output() filtersChange = new EventEmitter<any>();
  @Input() list: ListService;
  @Input() cars: PagedResultDto<CarDto> = { items: [], totalCount: 0 };

  @ViewChild('estReleaseModal', { static: true })
  estReleaseModal!: EstReleaseModal;

  @ViewChild('notesModal', { static: true })
  notesModal!: CarNotesModal;

  selectedCar = {} as CarDto;
  isRecallModalVisible = false;
  isCheckInModalVisible = false;
  isNotesModalVisible = false;

  openRecallModal(car: CarDto): void {
    this.selectedCar = car;
    this.isRecallModalVisible = true;
  }

  openCheckInModal(car: CarDto): void {
    this.selectedCar = car;
    this.isCheckInModalVisible = true;
  }

  openEstReleaseModal(row: CarDto): void {
    if (!row?.id) return;
    this.estReleaseModal.open(row.id, row.deliverDate ?? null);
  }

  onEstReleaseSaved(e: { carId: string; date: Date | null }): void {
    const row = this.cars.items?.find(x => x.id === e.carId);
    if (row) {
      (row as any).deliverDate = e.date;
    }
    this.list.get();
  }

   openNotesModal(row: CarDto): void {
    if (!row?.id) return;
    this.notesModal.open(row.id, row.notes);
  }

  onNotesSaved(e: { carId: string; notes: string }): void {
    const row = this.cars.items?.find(x => x.id === e.carId);
    if (row) row.notes = e.notes;
    this.list.get();
  }
}