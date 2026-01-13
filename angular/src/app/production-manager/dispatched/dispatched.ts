import { PagedResultDto, ListService } from '@abp/ng.core';
import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CheckInReportModal } from 'src/app/check-in-reports/check-in-report-modal/check-in-report-modal';
import { CarBayDto, Priority, avvStatusOptions } from 'src/app/proxy/car-bays';
import { CarService, CarDto } from 'src/app/proxy/cars';
import { StorageLocation } from 'src/app/proxy/cars/storage-locations';
import { LookupService, GuidLookupDto } from 'src/app/proxy/lookups';
import { Recalls } from 'src/app/recalls/recalls';
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';

@Component({
  selector: 'app-dispatched',
  imports: [...SHARED_IMPORTS, Recalls, CheckInReportModal],
  templateUrl: './dispatched.html',
  styleUrl: './dispatched.scss'
})
export class Dispatched {
private readonly carService = inject(CarService);
  private readonly lookupService = inject(LookupService);
  private readonly fb = inject(FormBuilder);
  private readonly toaster = inject(ToasterHelperService)


  form!: FormGroup;
  avvForm!: FormGroup;
  estReleaseForm!: FormGroup;
  StorageLocation = StorageLocation;

  @Input() cars: PagedResultDto<CarDto> = { items: [], totalCount: 0 };

  @Input() filters: any = {};
  @Output() filtersChange = new EventEmitter<any>();
  @Input() list: ListService;

  selectedCar = {} as CarDto;
  selectedId?: string;            // REMOVE THIS. Instead send the whole CarDto object

  isEstReleaseModalVisible = false;
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

  openEstReleaseModal(car: CarDto): void {
  this.selectedCar = car;

  const dateValue = car.deliverDate ? this.toDateInputValue(car.deliverDate) : null;

  this.estReleaseForm = this.fb.group({
    estimatedReleaseDate: [dateValue],
  });

  this.isEstReleaseModalVisible = true;
}

closeEstReleaseModal(): void {
  this.isEstReleaseModalVisible = false;
  this.estReleaseForm = undefined as any;
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

  saveEstRelease(): void {
  if (!this.selectedCar?.id) return;

  const dateStr = this.estReleaseForm.value.estimatedReleaseDate as string | null;
  if (!dateStr) return;

  const estimatedReleaseDate = new Date(dateStr);
    this.carService.updateEstimatedRelease(this.selectedCar.id, estimatedReleaseDate.toISOString()).subscribe(() => {
      this.isEstReleaseModalVisible = false;
      this.toaster.success('::EstimatedReleaseDateUpdated', '::Success');
      this.list.get();
    });
}

  private toDateInputValue(date: string | Date): string {
    const d = new Date(date);
    const yyyy = d.getFullYear();
    const mm = String(d.getMonth() + 1).padStart(2, '0');
    const dd = String(d.getDate()).padStart(2, '0');
    return `${yyyy}-${mm}-${dd}`;
  }
}
