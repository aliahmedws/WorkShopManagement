import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { CarService } from 'src/app/proxy/cars';
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';

@Component({
  selector: 'app-car-notes-modal',
  imports: [...SHARED_IMPORTS],
  templateUrl: './car-notes-modal.html',
  styleUrl: './car-notes-modal.scss'
})
export class CarNotesModal {
  private readonly fb = inject(FormBuilder);
  private readonly carService = inject(CarService);
  private readonly toaster = inject(ToasterHelperService);

  visible = false;
  saving = false;

  carId?: string;
  vin? : string;

  form?: FormGroup;

  // @Input() vin : string | null;
  /** emitted after successful save (so parent can refresh list/details) */
  @Output() saved = new EventEmitter<string>();

  /** for two-way binding if you want it */
  @Output() visibleChange = new EventEmitter<boolean>();

  open(carId: string, existingNotes?: string | null, vin?: string | null): void {
    this.vin = vin;
    this.carId = carId;

    this.form = this.fb.group({
      notes: [existingNotes ?? ''],
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

    const notes = (this.form.value?.notes ?? '') as string;

    this.saving = true;
    this.carService.updateNotes(this.carId, notes).subscribe({
      next: () => {
        this.saving = false;
        this.toaster.success('::NotesUpdatedSuccessfully', '::Success');
        this.saved.emit(this.carId!);
        this.close();
      },
      error: () => {
        this.saving = false;
      },
    });
  }
}
