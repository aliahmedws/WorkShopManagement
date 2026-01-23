import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { finalize } from 'rxjs';

import { SHARED_IMPORTS } from '../shared/shared-imports.constants';
import { RecallService, RecallDto, CreateRecallDto, UpdateRecallDto } from '../proxy/recalls';
import { RecallStatus } from '../proxy/recalls/recall-status.enum';
import { RecallType } from '../proxy/recalls/recall-type.enum';
import { ToasterHelperService } from '../shared/services/toaster-helper.service';

@Component({
  selector: 'app-recalls',
  imports: [...SHARED_IMPORTS],
  templateUrl: './recalls.html',
  styleUrl: './recalls.scss',
})
export class Recalls {
  private readonly recallService = inject(RecallService);
  private readonly fb = inject(FormBuilder);
  private readonly toaster = inject(ToasterHelperService);

  @Input() carId: string;
  @Input() vin: string;
  @Output() submit = new EventEmitter<boolean>();

  @Input() visible: boolean = false;
  @Output() visibleChange = new EventEmitter<boolean>();

  loading = true;
  saving = false;

  

  // recalls: RecallDto[] = [];
  // externalRecalls: ExternalRecallDetailDto[] = [];

  form = this.fb.group({
    rows: this.fb.array<FormGroup>([]),
  });

  readonly RecallStatus = RecallStatus;
  readonly RecallType = RecallType;

  modalOptions = {
    size: 'xl',
    backdrop: 'static',
    keyboard: false,
    animation: true,
  };

  get rows(): FormArray<FormGroup> {
    return this.form.get('rows') as FormArray<FormGroup>;
  }

  get(): void {
    if (!this.carId) return;

    this.loading = true;
    this.rows.clear();

    this.recallService.getListByCar(this.carId)
      .pipe(finalize(() => this.loading = false))
      .subscribe((recalls) => {
        this.buildRows(recalls ?? []);
      });
  }

  private buildRows(items: RecallDto[]): void {
    for (const r of items) {
      this.rows.push(
        this.fb.group({
          id: [r.id],
          carId: [this.carId], // Ensure carId is passed back
          title: [r.title ?? null, [Validators.required, Validators.maxLength(256)]],
          make: [r.make ?? null],
          manufactureId: [r.manufactureId ?? null],
          riskDescription: [r.riskDescription ?? null, Validators.maxLength(4000)],
          status: [r.status ?? RecallStatus.Pending, Validators.required],
          
          // Map Type enum to Boolean for Switch UI
          isCsp: [r.type === RecallType.Csps], 
          
          notes: [r.notes ?? null, Validators.maxLength(4000)],
          isExternal: [r.isExternal], // Used for Badge UI
          concurrencyStamp: [r.concurrencyStamp ?? null],

          // File Handling
          tempFiles: [r.tempFiles ?? []], 
          existingFiles: [r.entityAttachments ?? []]
        })
      );
    }
  }



  save(): void {
    if (this.loading || this.saving) return;

    this.rows.markAllAsTouched();
    if (this.rows.invalid) return;

    this.saving = true;

    // Prepare DTOs
    const formValues = this.rows.getRawValue();
    
    const dtos: RecallDto[] = formValues.map(v => ({
      ...v,
      // Map UI Boolean back to Enum
      type: v.isCsp ? RecallType.Csps : RecallType.Recalls,
      // Map files to DTO properties expected by backend
      isExternal: v.isExternal,
      tempFiles: v.tempFiles,
      entityAttachments: v.existingFiles
    }));

    this.recallService.addOrUpdateRecalls(this.carId, dtos)
      .pipe(finalize(() => this.saving = false))
      .subscribe({
        next: () => {
          this.toaster.createdOrUpdated('::RecallsSavedSuccessfully');
          this.submit.emit(true);
          this.close();
        }
      });
  }


  close(): void {
    this.visible = false;
    this.visibleChange.emit(this.visible);
  }


}
