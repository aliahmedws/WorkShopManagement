import { PagedResultDto, ListService } from '@abp/ng.core';
import { Component, EventEmitter, inject, Input, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CheckInReportModal } from 'src/app/check-in-reports/check-in-report-modal/check-in-report-modal';
import { CarBayService, CarBayDto, Priority, CreateCarBayDto } from 'src/app/proxy/car-bays';
import { CarDto } from 'src/app/proxy/cars';
import { StorageLocation } from 'src/app/proxy/cars/storage-locations';
import { LookupService, GuidLookupDto } from 'src/app/proxy/lookups';
import { Recalls } from 'src/app/recalls/recalls';
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { ProductionDetailsModal } from '../production/production-details-modal/production-details-modal';

@Component({
  selector: 'app-post-production',
  imports: [...SHARED_IMPORTS, Recalls, CheckInReportModal, ProductionDetailsModal],
  templateUrl: './post-production.html',
  styleUrl: './post-production.scss',
})
export class PostProduction {
  @ViewChild('detailsModal') detailsModal!: ProductionDetailsModal; //Bay

// private readonly carService = inject(CarService);
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
  selectedCarBay = {} as CarBayDto;

  priority = Priority;

  isRecallModalVisible = false;
  isCheckInModalVisible = false;
  // ngOnInit(): void {
  //   const carStreamCreator = (query: any) => this.carService.getList({ ...query, ...this.filters });
  //   this.list.hookToQuery(carStreamCreator).subscribe((res) => (this.cars = res));
  // }

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
      manufactureStartDate: [this.selectedCarBay.manufactureStartDate || null, [Validators.required]],
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

    const { manufactureStartDate, bayId, priority } = this.form.value;

    const input: CreateCarBayDto = {
      carId: this.selectedId,
      bayId,
      priority,
      isActive: true,
      manufactureStartDate
    };

    this.carBayService.create(input).subscribe(() => {
      this.toaster.assign();
      this.isAssignModalVisible = false;
      this.list.get();
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

  openProductionDetails(row: CarDto): void {
    if (!row.id) return;
    this.detailsModal.open(row.id, false, true);
}

onStageChanged(carId: string) {
  this.list.get();
  this.toaster.success('::SuccessfullyMovedToNextStage', '::Success');
}

}
