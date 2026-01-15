import { Component, inject, Input, Output, EventEmitter, OnChanges } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Confirmation, ToasterService } from '@abp/ng.theme.shared';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { ChoiceOptions } from 'src/app/proxy/utils/enums';
import { StorageLocation } from 'src/app/proxy/cars/storage-locations';
import { CheckInReportDto, CheckInReportService, CreateCheckInReportDto, UpdateCheckInReportDto } from 'src/app/proxy/check-in-reports';
import { CarDto, CarService, UpdateCarDto } from 'src/app/proxy/cars';
import { ConfirmationHelperService } from 'src/app/shared/services/confirmation-helper.service';
import { Stage } from 'src/app/proxy/cars/stages';
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';

@Component({
  selector: 'app-check-in-report-modal',
  standalone: true,
  imports: [ReactiveFormsModule, ThemeSharedModule, ...SHARED_IMPORTS],
  templateUrl: './check-in-report-modal.html',
  styleUrl: './check-in-report-modal.scss'
})
export class CheckInReportModal {

  private fb = inject(FormBuilder);
  private service = inject(CheckInReportService);
  private readonly carService = inject(CarService);
  private toaster = inject(ToasterHelperService);
  private readonly confirm = inject(ConfirmationHelperService);


  @Input() visible = false;
  @Output() visibleChange = new EventEmitter<boolean>();

  @Input() car: CarDto;
  @Output() submit = new EventEmitter<void>();

  form: FormGroup;
  existingReport = {} as CheckInReportDto;

  // Enums for Select Lists
  choiceOptions = Object.values(ChoiceOptions).filter(val => typeof val === 'number');
  storageLocationOptions = Object.values(StorageLocation).filter(val => typeof val === 'number');

  modalOptions = {
    size: 'lg',
    backdrop: 'static', //prevent close by outside click
    keyboard: false, //prevent close by esc key
    animation: true,
  };

  loading: boolean = false;

  constructor() {
    // Initialize empty form to avoid null errors before data loads
    this.buildForm();
  }

  public get() {
    this.loading = true;
    this.service.getByCarId(this.car.id)
      .subscribe((res: CheckInReportDto) => {
        this.existingReport = res
        this.buildForm(this.existingReport);
        this.loading = false;
      });
  }

  private buildForm(dto?: CheckInReportDto) {

    // Helper to format Date for <input type="date"> (YYYY-MM-DD)
    // const formatDate = (dateStr?: string) => dateStr ? dateStr.split('T')[0] : null;
    const carStorage = this.car ? this.car.storageLocation : null;

    this.form = this.fb.group({
      // carId: [this.carId, Validators.required],
      avcStickerCut: [dto?.avcStickerCut ?? null],
      avcStickerPrinted: [dto?.avcStickerPrinted ?? null],
      compliancePlatePrinted: [dto?.compliancePlatePrinted ?? null],
      // complianceDate: [formatDate(dto?.complianceDate)], 

      buildYear: [dto?.buildYear ?? null],
      buildMonth: [dto?.buildMonth ?? null],

      entryKms: [dto?.entryKms ?? null],
      engineNumber: [dto?.engineNumber ?? null],

      frontGwar: [dto?.frontGwar ?? null],
      rearGwar: [dto?.rearGwar ?? null],
      frontMotorNumber: [dto?.frontMoterNumber ?? null],
      rearMotorNumber: [dto?.rearMotorNumber ?? null],
      emission: [dto?.emission ?? null],
      maxTowingCapacity: [dto?.maxTowingCapacity ?? null],
      tyreLabel: [dto?.tyreLabel ?? null],
      // rsvaImportApproval: [dto?.rsvaImportApproval ?? null],
      reportStatus: [dto?.reportStatus ?? null],
      storageLocation: [dto?.storageLocation ?? carStorage],
      concurrencyStamp: [dto?.concurrencyStamp ?? null],
    });

  }

  save() {
    if (this.form.invalid) { return; }

    const formValue = this.form.value;
    const sl = formValue?.storageLocation;

    const request$ = this.existingReport?.id
      ? this.service.update(this.existingReport.id, formValue as UpdateCheckInReportDto)
      : this.service.create({ ...formValue as CreateCheckInReportDto, carId: this.car.id });

    request$.subscribe( () => {
      this.toaster.createdOrUpdated(this.car.id);
      this.submit.emit();
      this.close();
    });
  }

  close() {
    this.visible = false;
    this.visibleChange.emit(false);
  }
}