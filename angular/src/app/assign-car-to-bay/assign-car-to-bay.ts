import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';

import { LocalizationModule } from '@abp/ng.core';
import { ThemeSharedModule, ToasterService } from '@abp/ng.theme.shared';

export enum CarPriority {
  Low = 'Low',
  Medium = 'Medium',
  High = 'High',
  Urgent = 'Urgent',
}

export enum ServiceBay {
  Bay1 = 'Bay 1',
  Bay2 = 'Bay 2',
  Bay3 = 'Bay 3',
  Bay4 = 'Bay 4',
  Bay5 = 'Bay 5',
  Bay6 = 'Bay 6',
  Bay7 = 'Bay 7',
  Bay8 = 'Bay 8',
}

@Component({
  selector: 'app-assign-car-to-bay',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    LocalizationModule,
    ThemeSharedModule,
  ],
  templateUrl: './assign-car-to-bay.html',
  styleUrls: ['./assign-car-to-bay.scss'],
})
export class AssignCarToBay implements OnInit {
  priorities = Object.values(CarPriority);
  bays = Object.values(ServiceBay);

  form = this.fb.group({
    vinNumber: this.fb.control({ value: '', disabled: true }),
    priority: this.fb.control<CarPriority | null>(null, Validators.required),
    bay: this.fb.control<ServiceBay | null>(null, Validators.required),
    dueDate: this.fb.control<string | null>(null),
  });

  constructor(
    private readonly fb: FormBuilder,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly toaster: ToasterService
  ) {}

  ngOnInit(): void {
    const vin =
      this.route.snapshot.queryParamMap.get('vin') ??
      (history.state?.vin as string | undefined);

    if (vin) {
      this.form.patchValue({ vinNumber: vin.trim().toUpperCase() }, { emitEvent: false });
    }
  }

  back(): void {
    this.router.navigateByUrl('/cars');
  }

  clear(): void {
    const vin = this.form.getRawValue().vinNumber;
    this.form.reset(
      {
        vinNumber: vin,
        priority: null,
        bay: null,
        dueDate: null,
      },
      { emitEvent: false }
    );
    this.form.get('vinNumber')?.disable({ emitEvent: false });
  }

  assign(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const payload = this.form.getRawValue(); // includes disabled vinNumber
    this.toaster.success(`Assigned ${payload.vinNumber} to ${payload.bay} (${payload.priority}).`);
    this.back();
  }
}
