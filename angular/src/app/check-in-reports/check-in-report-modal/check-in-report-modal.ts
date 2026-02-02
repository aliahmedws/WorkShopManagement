import { Component, inject, Input, Output, EventEmitter, OnChanges, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Confirmation, ToasterService } from '@abp/ng.theme.shared';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { choiceOptionsOptions } from 'src/app/proxy/utils/enums';
import { storageLocationOptions } from 'src/app/proxy/cars/storage-locations';
import { CheckInReportDto, CheckInReportService, CreateCheckInReportDto, UpdateCheckInReportDto } from 'src/app/proxy/check-in-reports';
import { CarDto, CarService } from 'src/app/proxy/cars';
import { ConfirmationHelperService } from 'src/app/shared/services/confirmation-helper.service';
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';
import { LookupService } from 'src/app/proxy/lookups';
import { SpecAttributesDto, SpecsResponseDto } from 'src/app/proxy/external/cars-xe';
import { PermissionService } from '@abp/ng.core';

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
  private toaster = inject(ToasterHelperService);
  private permission = inject(PermissionService);

  private readonly lookupService = inject(LookupService);

  hasPermission = false;

  @Input() visible = false;
  @Output() visibleChange = new EventEmitter<boolean>();

  @Input() carId?: string;
  @Input() carVin?: string;
  @Output() submit = new EventEmitter<void>();

  specAttributes: SpecAttributesDto | null = null;

  form: FormGroup;
  existingReport = {} as CheckInReportDto;

  // Enums for Select Lists
  choiceOptions = choiceOptionsOptions;
  storageLocationOptions = storageLocationOptions;

  modalOptions = {
    size: 'lg',
    backdrop: 'static', //prevent close by outside click
    keyboard: false, //prevent close by esc key
    animation: true,
  };

  constructor() {
    this.buildForm();
    this.hasPermission =
      this.permission.getGrantedPolicy(
        'WorkShopManagement.CheckInReports'
      );
  }

  public get() {
    if (!this.hasPermission || !this.carId) return;

    this.specAttributes = {} as SpecAttributesDto;

    this.service.getByCarId(this.carId)
      .subscribe((res: CheckInReportDto) => {
        this.existingReport = res
        this.buildForm(this.existingReport);
        this.fetchSpecs();
      });
  }

  fetchSpecs() {
    if (!this.hasPermission || !this.carVin) return;

    this.lookupService.getExternalSpecsResponse(this.carVin).subscribe((res: SpecsResponseDto) => {
      if (res && res.success) {
        this.specAttributes = res.attributes;

        const v = this.form.value;
        this.form.patchValue({
          buildYear: v.buildYear ?? res.attributes?.year,
          maxTowingCapacity: v.maxTowingCapacity ?? this.getWeigthValue(res.attributes?.maximum_towing),
          tyreLabel: v.tyreLabel ?? res.attributes?.tires,
        });
      }
    });
  }

  private buildForm(dto?: CheckInReportDto) {
    // const carStorage = this.existingReport.car ? this.existingReport.car.storageLocation : null;

    this.form = this.fb.group({
      // carId: [this.carId, Validators.required],
      avcStickerCut: [dto?.avcStickerCut ?? null],
      avcStickerPrinted: [dto?.avcStickerPrinted ?? null],
      compliancePlatePrinted: [dto?.compliancePlatePrinted ?? null],
      // complianceDate: [formatDate(dto?.complianceDate)], 

      buildYear: [dto?.buildYear ?? (this.specAttributes?.year ?? null)],
      buildMonth: [dto?.buildMonth ?? null],

      entryKms: [dto?.entryKms ?? null],
      engineNumber: [dto?.engineNumber ?? null],

      frontGawr: [dto?.frontGawr ?? null],
      rearGawr: [dto?.rearGawr ?? null],
      frontMotorNumber: [dto?.frontMotorNumber ?? null],
      rearMotorNumber: [dto?.rearMotorNumber ?? null],
      emission: [dto?.emission ?? null],
      maxTowingCapacity: [dto?.maxTowingCapacity ?? (this.getWeigthValue(this.specAttributes?.maximum_towing) ?? null)],
      tyreLabel: [dto?.tyreLabel ?? (this.specAttributes?.tires ?? null)],
      // rvsaImportApproval: [dto?.rvsaImportApproval ?? null],
      reportStatus: [dto?.reportStatus ?? null],
      storageLocation: [dto?.storageLocation ?? null],
      concurrencyStamp: [dto?.concurrencyStamp ?? null],
    });

  }

  save() {
    if (this.form.invalid) { return; }

    const formValue = this.form.value;

    const request$ = this.existingReport?.id
      ? this.service.update(this.existingReport.id, formValue as UpdateCheckInReportDto)
      : this.service.create({ ...formValue as CreateCheckInReportDto, carId: this.carId });

    request$.subscribe(() => {
      this.toaster.createdOrUpdated(this.carId);
      this.submit.emit();
      this.close();
    });
  }

  close() {
    this.visible = false;
    this.visibleChange.emit(false);
  }



  getWeigthUnit(field: string | null): string | null {
    if (!field) return null;

    const raw = field.trim();
    if (!raw) return null;

    // Remove commas to simplify parsing.
    const s = raw.replace(/,/g, '');

    // Common case: "5000 lbs", "18.50 gallon", "57.60 in."
    const m = s.match(/^\s*[-+]?\d*\.?\d+\s*([^\d]+)\s*$/);
    if (m?.[1]) {
      const unit = m[1].trim();
      return unit.length ? unit : null;
    }

    // Range formats: "19 - 21 miles/gallon"
    // In this case, unit is after the last number segment.
    const range = s.match(/^\s*[-+]?\d*\.?\d+\s*-\s*[-+]?\d*\.?\d+\s*([^\d]+)\s*$/);
    if (range?.[1]) {
      const unit = range[1].trim();
      return unit.length ? unit : null;
    }

    // If we can't confidently detect a unit, return null.
    return null;
  }

  getWeigthValue(field: string | null): number | null {
    if (!field) return null;

    const raw = field.trim();
    if (!raw) return null;

    // Remove commas to support "5,000 lbs"
    const s = raw.replace(/,/g, '');

    // If it's a range, take the first number as the "value" (you can change to avg if you prefer).
    const range = s.match(/^\s*([-+]?\d*\.?\d+)\s*-\s*([-+]?\d*\.?\d+)/);
    if (range?.[1]) {
      const n = Number(range[1]);
      return Number.isFinite(n) ? n : null;
    }

    // Standard: take the first numeric token
    const m = s.match(/[-+]?\d*\.?\d+/);
    if (!m) return null;

    const n = Number(m[0]);
    return Number.isFinite(n) ? n : null;
  }
}