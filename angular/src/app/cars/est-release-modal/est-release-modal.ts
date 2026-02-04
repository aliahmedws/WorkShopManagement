import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { CarService } from 'src/app/proxy/cars';
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { NgbDateAdapter, NgbDateNativeAdapter } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-est-release-modal',
  standalone: true,
  imports: [...SHARED_IMPORTS],
  templateUrl: './est-release-modal.html',
  styleUrl: './est-release-modal.scss',
  providers: [{ provide: NgbDateAdapter, useClass: NgbDateNativeAdapter }],
})
export class EstReleaseModal {
  private readonly fb = inject(FormBuilder);
  private readonly carService = inject(CarService);
  private readonly toaster = inject(ToasterHelperService);

  visible = false;
  saving = false;

  carId?: string;
  form?: FormGroup;

  vin?: string;
  @Output() saved = new EventEmitter<{ carId: string; date: Date | null }>();
  @Output() visibleChange = new EventEmitter<boolean>();

  open(carId: string, existingDate?: Date | string | null, vin?: string | null,): void {
    this.carId = carId;
    this.vin = vin;

    const date =
      existingDate == null
        ? null
        : existingDate instanceof Date
          ? existingDate
          : new Date(existingDate);

    this.form = this.fb.group({
      estimatedReleaseDate: [existingDate ? new Date(existingDate) : null],
    });

    this.visible = true;
    this.visibleChange.emit(true);
  }

  close(): void {
    this.visible = false;
    this.visibleChange.emit(false);

    this.carId = undefined;
    this.form = undefined;
    this.saving = false;
  }

  save(): void {
    if (!this.carId || !this.form || this.saving) return;

    const dateObj = (this.form.value?.estimatedReleaseDate ?? null) as Date | null;

    const dateStr: string | null = dateObj
      ? `${dateObj.getFullYear()}-${String(dateObj.getMonth() + 1).padStart(2, '0')}-${String(dateObj.getDate()).padStart(2, '0')}`
      : null;

    this.saving = true;

    this.carService.updateEstimatedRelease(this.carId, dateStr).subscribe({
        next: () => {
          this.saving = false;
          this.toaster.success('::EstimatedReleaseUpdatedSuccessfully', '::Success');
          this.saved.emit({ carId: this.carId!, date: dateObj });
          this.close();
        },
        error: () => {
          this.saving = false;
        },
      });
  }

}