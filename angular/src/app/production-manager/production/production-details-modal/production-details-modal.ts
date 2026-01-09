import { Component, EventEmitter, Output, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { CarBayDto, CarBayService, Priority } from 'src/app/proxy/car-bays';

@Component({
  selector: 'app-production-details-modal',
  standalone: true,
  imports: [...SHARED_IMPORTS],
  templateUrl: './production-details-modal.html',
  styleUrls: ['./production-details-modal.scss'],
})
export class ProductionDetailsModal {
  private readonly carBayService = inject(CarBayService);
  private readonly fb = inject(FormBuilder);

  visible = false;
  @Output() visibleChange = new EventEmitter<boolean>();

  carBayId?: string;
  details?: CarBayDto;

  Priority = Priority;

  form?: FormGroup;

  open(id: string): void {
    this.carBayId = id;
    this.visible = true;
    this.visibleChange.emit(true);

    this.loadDetails();
  }

  close(): void {
    this.visible = false;
    this.visibleChange.emit(false);
    this.details = undefined;
    this.form = undefined;
    this.carBayId = undefined;
  }

  private loadDetails(): void {
    if (!this.carBayId) return;

    // IMPORTANT: this must exist in proxy (recommended)
    this.carBayService.get(this.carBayId).subscribe((res: CarBayDto) => {
      this.details = res;
      this.buildForm();
    });
  }

  private buildForm(): void {
    // If you have quality gates fields in CarBayDto, bind them here
    // Otherwise remove form completely.
    this.form = this.fb.group({
      // example only, replace with your real fields:
      // preProduction: [this.details?.preProduction ?? null, Validators.required],
    });
  }

  normalizeUrl(url?: string | null): string {
    if (!url) return '';
    // return url.replaceAll('\\', '/');
  }

  vinLast6(v?: string | null): string {
    if (!v) return '-';
    return v.length > 6 ? v.slice(-6) : v;
  }
}
