import { Component, EventEmitter, inject, Output } from '@angular/core';
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
  form?: FormGroup;

  /** emitted after successful save so parent can update UI */
  @Output() saved = new EventEmitter<{ carId: string; notes: string }>();

  open(carId: string, existingNotes?: string | null): void {
    this.carId = carId;

    this.form = this.fb.group({
      notes: [existingNotes ?? ''],
    });

    this.visible = true;
  }

  close(): void {
    this.visible = false;
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
        this.toaster.success('::NotesUpdatedSuccessfully', '::Success');
        this.saved.emit({ carId: this.carId!, notes });
        this.close();
      },
      error: () => {
        this.saving = false;
      },
      complete: () => {
        this.saving = false;
      },
    });
  }
}
