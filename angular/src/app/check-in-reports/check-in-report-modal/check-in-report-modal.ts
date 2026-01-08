import { Component, inject, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ToasterService } from '@abp/ng.theme.shared';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { finalize } from 'rxjs/operators';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { ChoiceOptions } from 'src/app/proxy/utils/enums';
import { StorageLocation } from 'src/app/proxy/cars/storage-locations';
import { CheckInReportDto, CheckInReportService } from 'src/app/proxy/check-in-reports';

@Component({
  selector: 'app-check-in-report-modal',
  standalone: true,
  imports: [ReactiveFormsModule, ThemeSharedModule, ...SHARED_IMPORTS],
  templateUrl: './check-in-report-modal.html',
  styleUrl: './check-in-report-modal.scss'
})
export class CheckInReportModal implements OnChanges {

  private fb = inject(FormBuilder);
  private service = inject(CheckInReportService);
  private toaster = inject(ToasterService);

  @Input() visible = false;
  @Output() visibleChange = new EventEmitter<boolean>();
  
  @Input() carId: string = '';
  @Output() submit = new EventEmitter<void>();

  isBusy = false;
  form: FormGroup;
  existingReportId: string | null = null;

  // Enums for Select Lists
  choiceOptions = Object.values(ChoiceOptions).filter(val => typeof val === 'number');
  storageLocationOptions = Object.values(StorageLocation).filter(val => typeof val === 'number');

  constructor() {
    // Initialize empty form to avoid null errors before data loads
    this.buildForm(); 
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.visible && changes.visible.currentValue === true && this.carId) {
      this.fetchData();
    }
  }

  private fetchData() {
    this.isBusy = true;
    this.existingReportId = null;

    this.service.getByCarId(this.carId)
      .pipe(finalize(() => this.isBusy = false))
      .subscribe({
        next: (result) => {
          this.existingReportId = result ? result.id : null;
          // PASS RESULT TO BUILD FORM (Or undefined if null)
          this.buildForm(result || undefined);
        },
        error: () => {
          this.existingReportId = null;
          this.buildForm(); // Rebuild empty form on error/404
        }
      });
  }

  // UPDATED: Now accepts optional DTO
  private buildForm(dto?: CheckInReportDto) {
    
    // Helper to format Date for <input type="date"> (YYYY-MM-DD)
    const formatDate = (dateStr?: string) => dateStr ? dateStr.split('T')[0] : null;

    this.form = this.fb.group({
      buildYear: [dto?.buildYear ?? null],
      buildMonth: [dto?.buildMonth ?? null],
      
      // Handle Date formatting
      complianceDate: [formatDate(dto?.complianceDate)], 

      entryKms: [dto?.entryKms ?? null],
      engineNumber: [dto?.engineNumber ?? null],
      emission: [dto?.emission ?? null],
      vinNo: [dto?.vinNo ?? null], 
      modelName: [dto?.modelName ?? null],
      
      // --- ENUMS WITH DEFAULTS ---
      // Pattern: dto?.property ?? DefaultValue
      
      // Default to null (Select...) if new, or use existing value
      avcStickerCut: [dto?.avcStickerCut ?? null], 
      avcStickerPrinted: [dto?.avcStickerPrinted ?? null],
      compliancePlatePrinted: [dto?.compliancePlatePrinted ?? null],
      
      // Example: If you wanted a specific default like 'Yard' (assuming enum value 1)
      // storageLocation: [dto?.storageLocation ?? StorageLocation.Yard],
      storageLocation: [dto?.storageLocation ?? null],

      frontGwar: [dto?.frontGwar ?? null],
      frontMoterNumber: [dto?.frontMoterNumber ?? null],
      rearGwar: [dto?.rearGwar ?? null],
      rearMotorNumber: [dto?.rearMotorNumber ?? null],
      maxTowingCapacity: [dto?.maxTowingCapacity ?? null],
      tyreLabel: [dto?.tyreLabel ?? null],
      rsvaImportApproval: [dto?.rsvaImportApproval ?? null],
      reportStatus: [dto?.reportStatus ?? null],
    });
  }

  save() {
    if (this.form.invalid) { return; }

    this.isBusy = true;
    const formValue = this.form.value;

    const request$ = this.existingReportId
      ? this.service.update(this.existingReportId, formValue)
      : this.service.create({ ...formValue, carId: this.carId });

    request$
      .pipe(finalize(() => this.isBusy = false))
      .subscribe(() => {
        this.toaster.success('::SuccessfullySaved');
        this.submit.emit();
        this.close();
      });
  }

  close() {
    this.visible = false;
    this.visibleChange.emit(false);
  }
}