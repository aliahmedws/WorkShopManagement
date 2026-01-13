import { PagedResultDto, ListService } from '@abp/ng.core';
import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CheckInReportModal } from 'src/app/check-in-reports/check-in-report-modal/check-in-report-modal';
import { CarBayService, CarBayDto, Priority, avvStatusOptions } from 'src/app/proxy/car-bays';
import { CarDto, CarService } from 'src/app/proxy/cars';
import { StorageLocation } from 'src/app/proxy/cars/storage-locations';
import { LookupService, GuidLookupDto } from 'src/app/proxy/lookups';
import { Recalls } from 'src/app/recalls/recalls';
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';

@Component({
  selector: 'app-awaiting-transport',
  imports: [...SHARED_IMPORTS, Recalls, CheckInReportModal],
  templateUrl: './awaiting-transport.html',
  styleUrl: './awaiting-transport.scss'
})
export class AwaitingTransport {
 private readonly carService = inject(CarService);
  private readonly lookupService = inject(LookupService);
  private readonly fb = inject(FormBuilder);
  private readonly toaster = inject(ToasterHelperService)


  form!: FormGroup;
  avvForm!: FormGroup;
  StorageLocation = StorageLocation;

  @Input() cars: PagedResultDto<CarDto> = { items: [], totalCount: 0 };

  @Input() filters: any = {};
  @Output() filtersChange = new EventEmitter<any>();
  @Input() list: ListService;

  selectedCar = {} as CarDto;
  selectedId?: string;            // REMOVE THIS. Instead send the whole CarDto object

  isAssignModalVisible = false;
  isAvvModalVisible = false;

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

    this.avvForm = this.fb.group({
      avvStatus: [car.avvStatus ?? null, [Validators.required]],
    });

    this.isAvvModalVisible = true;
  }

  closeAvvModal(): void {
    this.isAvvModalVisible = false;
    this.avvForm = undefined as any;
  }

  saveAvvStatus(): void {
    if (!this.selectedCar?.id) return;

    this.avvForm.markAllAsTouched();
    if (this.avvForm.invalid) return;

    const avvStatus = this.avvForm.value.avvStatus;

    this.carService.updateAvvStatus(this.selectedCar.id, { avvStatus }).subscribe(() => {
      this.isAvvModalVisible = false;
      this.toaster.success('::AVVStatusUpdated', '::Success');
      this.list.get();
    });
  }
}
