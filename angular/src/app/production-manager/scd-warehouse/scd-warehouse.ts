import { PagedResultDto, ListService } from '@abp/ng.core';
import { Confirmation } from '@abp/ng.theme.shared';
import { Component, EventEmitter, inject, Input, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CheckInReport } from 'src/app/check-in-reports/check-in-report';
import { CheckInReportModal } from 'src/app/check-in-reports/check-in-report-modal/check-in-report-modal';
import { CarBayService, priorityOptions, CarBayDto, Priority, CreateCarBayDto } from 'src/app/proxy/car-bays';
import { CarService, CarDto } from 'src/app/proxy/cars';
import { Stage } from 'src/app/proxy/cars/stages';
import { StorageLocation } from 'src/app/proxy/cars/storage-locations/storage-location.enum';
import { LookupService, GuidLookupDto } from 'src/app/proxy/lookups';
import { Recalls } from 'src/app/recalls/recalls';
import { ConfirmationHelperService } from 'src/app/shared/services/confirmation-helper.service';
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { ProductionActions } from '../production-actions/production-actions';
import { EstReleaseModal } from "src/app/cars/est-release-modal/est-release-modal";

@Component({
  selector: 'app-scd-warehouse',
  imports: [...SHARED_IMPORTS, Recalls, CheckInReportModal, ProductionActions, EstReleaseModal],
  templateUrl: './scd-warehouse.html',
  styleUrl: './scd-warehouse.scss'
})
export class ScdWarehouse {
  private readonly carService = inject(CarService);
  private readonly confirm = inject(ConfirmationHelperService);
  private readonly carBayService = inject(CarBayService)
  private readonly lookupService = inject(LookupService);
  private readonly fb = inject(FormBuilder);
  private readonly toaster = inject(ToasterHelperService)


  form!: FormGroup;
  StorageLocation = StorageLocation;

  @Input() cars: PagedResultDto<CarDto> = { items: [], totalCount: 0 };


  @Input() filters: any = {};
  @Output() filtersChange = new EventEmitter<any>();
  @Input() list: ListService;

  selectedCar = {} as CarDto;
  selectedId?: string;            // REMOVE THIS. Instead send the whole CarDto object

  isAssignModalVisible = false;

  bayOptions: GuidLookupDto[] = [];
  priorityOptions = priorityOptions;
  selectedCarBay = {} as CarBayDto;

  priority = Priority;

  isRecallModalVisible = false;
  isCheckInModalVisible = false;
  
  @ViewChild('estReleaseModal', { static: true })
  estReleaseModal!: EstReleaseModal;

  loadBays() {
    if (!this.bayOptions.length) {
      this.lookupService
        .getBays()
        .subscribe(res => {
          this.bayOptions = res;
        })
    }
  }

  private buildForm(): void {
    this.form = this.fb.group({
      bayId: [this.selectedCarBay.bayId || null, [Validators.required]],
      priority: [this.selectedCarBay.priority || Priority.Medium, [Validators.required]],
    });
  }

  openAssignModal(carId: string): void {
    this.selectedId = carId;
    this.loadBays();
    this.buildForm();
    this.isAssignModalVisible = true;
  }

  closeAssignModal(): void {
    this.isAssignModalVisible = false;
    // this.selectedCarBay = {};
    this.selectedId = undefined;
  }

  assignToBay(): void {
    if (!this.selectedId) return;

    this.form.markAllAsTouched();
    if (this.form.invalid) return;

    this.confirm
      .confirmAction('::ConfirmAssignToBayMessage', '::ConfirmAssignToBayTitle')
      .subscribe(status => {
        if (status !== Confirmation.Status.confirm) return;

        const { bayId, priority } = this.form.value;

        const input: CreateCarBayDto = {
          carId: this.selectedId!,
          bayId,
          priority,
          isActive: true
        };

        this.carBayService.create(input).subscribe(() => {
          this.carService.changeStage(this.selectedId!, { targetStage: Stage.Production }).subscribe(() => {
            this.toaster.assign();
            this.list.get();
            this.isAssignModalVisible = false;
          });
        });
      });
  }


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
    const existing = row.deliverDate ? new Date(row.deliverDate) : null; // ✅ string -> Date
    this.estReleaseModal.open(row.id, existing);
  }

  onEstReleaseSaved(e: { carId: string; date: Date | null }): void {
    const row = this.cars.items?.find(x => x.id === e.carId);
    if (row) {
      (row as any).deliverDate = e.date ? e.date.toISOString() : null; // ✅ Date -> string
    }
    this.list.get();
  }
}
