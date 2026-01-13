import { ListService, PagedResultDto } from '@abp/ng.core';
import { Confirmation } from '@abp/ng.theme.shared';
import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CheckInReportModal } from 'src/app/check-in-reports/check-in-report-modal/check-in-report-modal';
import { CarBayDto, CarBayService, CreateCarBayDto, Priority, priorityOptions } from 'src/app/proxy/car-bays';
import { CarService, CarDto } from 'src/app/proxy/cars';
import { Stage } from 'src/app/proxy/cars/stages';
import { StorageLocation } from 'src/app/proxy/cars/storage-locations';
import { GuidLookupDto, LookupService } from 'src/app/proxy/lookups';
import { Recalls } from 'src/app/recalls/recalls';
import { ConfirmationHelperService } from 'src/app/shared/services/confirmation-helper.service';
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';



@Component({
  selector: 'app-external-warehouse',
  imports: [...SHARED_IMPORTS, Recalls, CheckInReportModal],
  templateUrl: './external-warehouse.html',
  styleUrl: './external-warehouse.scss'
})
export class ExternalWarehouse {
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
}