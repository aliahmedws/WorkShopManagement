import { Component, EventEmitter, inject, Input, Output, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CarDto, CarService } from 'src/app/proxy/cars';
import { CheckInReportDto, CheckInReportService } from 'src/app/proxy/check-in-reports';
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { NgbDateNativeAdapter, NgbDateAdapter } from '@ng-bootstrap/ng-bootstrap';
import { GuidLookupDto, LookupService } from 'src/app/proxy/lookups';

@Component({
  selector: 'app-create-check-in-report-modal',
  imports: [...SHARED_IMPORTS],
  templateUrl: './create-check-in-report-modal.html',
  styleUrl: './create-check-in-report-modal.scss',
  providers: [{provide: NgbDateAdapter, useClass: NgbDateNativeAdapter}]
})
export class CreateCheckInReportModal {
private readonly fb = inject(FormBuilder);
  private readonly carService = inject(CarService);
  private readonly checkInReportService = inject(CheckInReportService);
  private readonly toaster = inject(ToasterHelperService);
  private readonly lookupService = inject(LookupService);

@Input() checkInReportId?: string;
  @Output() submit = new EventEmitter<CheckInReportDto>();

  @Input() visible: boolean = false;
  @Output() visibleChange = new EventEmitter<boolean>();
  form: FormGroup;

   modalOptions = {
    size: 'lg',
    backdrop: 'static', //prevent close by outside click
    keyboard: false, //prevent close by esc key
    animation: true,
  };
  loading: boolean;
  carOptions: GuidLookupDto[] = [];
  isEditMode = false;
busy = false;

   private loadCars(): void {

    if(!this.carOptions?.length) {
      this.lookupService.getCars().subscribe(response => {
        this.carOptions = response;
      })
    }
  }

  ngOnChanges(changes: SimpleChanges) {
  // When modal is opened
  if (changes['visible']?.currentValue === true) {
    this.opened();
  }
}

private opened() {
   if (!this.form) return;
  this.isEditMode = !!this.checkInReportId;

  if (!this.isEditMode) {
    this.resetFormForCreate();
    return;
  }

  this.loadForEdit(this.checkInReportId!);
}

private loadForEdit(id: string) {
  this.busy = true;

  this.checkInReportService.get(id).subscribe({
    next: (dto) => {
      // patch form with dto fields
      this.form.patchValue(dto);
      this.form.markAsPristine();
    },
    error: () => {
      // optionally close modal or show a toast
    },
    complete: () => (this.busy = false),
  });
}

private resetFormForCreate() {
  this.form.reset();
  this.form.markAsPristine();
}

    onAppear(): void {
    // Called when modal appears; good place to fetch dropdown data and init form
    this.loadCars();
    this.buildForm();
    // If you support edit later, also load report by id here
  }

  buildForm(dto?: CheckInReportDto): void {
    this.form = this.fb.group({
      carId: [dto?.carId ?? null, [Validators.required]],
      vinNo: [dto?.vinNo ?? null, [Validators.required]],

      avcStickerCut: [dto?.avcStickerCut ?? false],
      avcStickerPrinted: [dto?.avcStickerPrinted ?? false],
      buildDate: [this.toDateInputValue(dto?.buildDate)],

      checkInSumbitterUser: [dto?.checkInSumbitterUser ?? null],

      complianceDate: [this.toDateInputValue(dto?.complianceDate)],
      compliancePlatePrinted: [dto?.compliancePlatePrinted ?? false],

      emission: [dto?.emission ?? null],
      engineNumber: [dto?.engineNumber ?? null],

      entryKms: [dto?.entryKms ?? null],
      frontGwar: [dto?.frontGwar ?? null],
      frontMoterNumbr: [dto?.frontMoterNumbr ?? null],
      rearGwar: [dto?.rearGwar ?? null],
      rearMotorNumber: [dto?.rearMotorNumber ?? null],

      hsObjectId: [dto?.hsObjectId ?? null],
      maxTowingCapacity: [dto?.maxTowingCapacity ?? 0], // backend is non-nullable float
      tyreLabel: [dto?.tyreLabel ?? null],

      rsvaImportApproval: [dto?.rsvaImportApproval ?? null],
      status: [dto?.status ?? null],
      model: [dto?.model ?? null],
      storageLocation: [dto?.storageLocation ?? null],
    });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const input = this.form.value;
    this.busy = true;


   const request$ = this.isEditMode
    ? this.checkInReportService.update(this.checkInReportId!, input)
    : this.checkInReportService.create(input);

  request$.subscribe({
    next: () => {
       this.toaster.createdOrUpdated(this.checkInReportId);
      this.submit.emit();
      this.close();
    },
    complete: () => (this.busy = false),
  });
}

close() {
  this.visible = false;
  this.visibleChange.emit(false);
}

  // Helpers for <input type="date"> which uses yyyy-MM-dd strings
  private toDateInputValue(value?: string | Date | null): string | null {
    if (!value) return null;
    const d = typeof value === 'string' ? new Date(value) : value;
    if (isNaN(d.getTime())) return null;

    const yyyy = d.getFullYear();
    const mm = String(d.getMonth() + 1).padStart(2, '0');
    const dd = String(d.getDate()).padStart(2, '0');
    return `${yyyy}-${mm}-${dd}`;
  }

  private fromDateInputValue(value: any): Date | null {
    if (!value) return null;
    // value expected as 'yyyy-MM-dd'
    const d = new Date(value);
    return isNaN(d.getTime()) ? null : d;
  }

}
