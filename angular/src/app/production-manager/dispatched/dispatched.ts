import { PagedResultDto, ListService } from '@abp/ng.core';
import { Component, EventEmitter, inject, Input, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CheckInReportModal } from 'src/app/check-in-reports/check-in-report-modal/check-in-report-modal';
import { CarBayDto, Priority, avvStatusOptions } from 'src/app/proxy/car-bays';
import { CarService, CarDto } from 'src/app/proxy/cars';
import { StorageLocation } from 'src/app/proxy/cars/storage-locations';
import { LookupService, GuidLookupDto } from 'src/app/proxy/lookups';
import { Recalls } from 'src/app/recalls/recalls';
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { ProductionActions } from '../production-actions/production-actions';
import { AvvStatusModal } from "../mini-modals/avv-status-modal/avv-status-modal";
import { EstReleaseModal } from "src/app/cars/est-release-modal/est-release-modal";

@Component({
  selector: 'app-dispatched',
  imports: [...SHARED_IMPORTS, Recalls, CheckInReportModal, ProductionActions, AvvStatusModal, EstReleaseModal],
  templateUrl: './dispatched.html',
  styleUrl: './dispatched.scss'
})
export class Dispatched {
  @ViewChild('estReleaseModal', { static: true })
  estReleaseModal!: EstReleaseModal;

  private readonly lookupService = inject(LookupService);

  form!: FormGroup;
  StorageLocation = StorageLocation;
  isAvvModalVisible = false;

  @Input() cars: PagedResultDto<CarDto> = { items: [], totalCount: 0 };

  @Input() filters: any = {};
  @Output() filtersChange = new EventEmitter<any>();
  @Input() list: ListService;

  selectedCar = {} as CarDto;
  selectedId?: string;            // REMOVE THIS. Instead send the whole CarDto object

  isAssignModalVisible = false;

  bayOptions: GuidLookupDto[] = [];
  selectedCarBay = {} as CarBayDto;

  priority = Priority;
  aVVStatusOptions = avvStatusOptions;

  isRecallModalVisible = false;
  isCheckInModalVisible = false;


  loadBays() {
    if (!this.bayOptions.length) {
      this.lookupService
        .getBays()
        .subscribe(res => {
          this.bayOptions = res;
        })
    }
  }

  openRecallModal(car: CarDto): void {
    this.selectedCar = car;
    this.isRecallModalVisible = true;
  }

  openCheckInModal(car: CarDto): void {
    this.selectedCar = car;
    this.isCheckInModalVisible = true;
  }

openAvvModal(car: CarDto): void {
    this.selectedCar = car;
    this.isAvvModalVisible = true;
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

}
