import { Component, EventEmitter, inject, Input, Output, ViewChild } from '@angular/core';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { QualityGates } from '../quality-gates/quality-gates';
import { finalize } from 'rxjs';
import { CarBayService } from 'src/app/proxy/car-bays';

@Component({
  selector: 'app-quality-gates-modal',
  imports: [...SHARED_IMPORTS, QualityGates],
  templateUrl: './quality-gates-modal.html',
  styleUrl: './quality-gates-modal.scss',
})
export class QualityGatesModal {
  private readonly carBayService = inject(CarBayService);

  @Input() visible = false;
  @Output() visibleChange = new EventEmitter<boolean>();

  @Input() carId?: string;
  @Input() vin?: string;
  resolvedCarBayId?: string;

  @Output() saved = new EventEmitter<void>();

  @ViewChild(QualityGates) gatesCmp?: QualityGates;

  loading = false;

  onVisibleChange(v: boolean): void {
    this.visible = v;
    this.visibleChange.emit(v);

    if (!v) {
      // reset local state only (not @Input)
      this.resolvedCarBayId = undefined;
      this.loading = false;
    }
  }

  // called when modal appears (each time it opens)
  onAppear(): void {
    if (!this.visible || !this.carId) return;

    this.loading = true;
    this.resolvedCarBayId = undefined;

    // Get latest CarBay for this car
    this.carBayService
      .get(this.carId)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (res: any) => {
          this.resolvedCarBayId = res?.id; // CarBayDto.id
        },
        error: () => {
          this.resolvedCarBayId = undefined;
        },
      });
  }

  close(): void {
    this.onVisibleChange(false);
  }

  onSaved(): void {
    this.saved.emit();
  }
}
