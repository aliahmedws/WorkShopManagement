import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';

import { LocalizationModule } from '@abp/ng.core';
import { ThemeSharedModule, ToasterService } from '@abp/ng.theme.shared';
import { CarsStorage } from '../models/cars.storage';



const VIN_REGEX = /^[A-HJ-NPR-Z0-9]{17}$/i;

export enum PriorityEnum {
  Low = 'Low',
  Medium = 'Medium',
  High = 'High',
  Urgent = 'Urgent',
}

export enum BayEnum {
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
  priorities = Object.values(PriorityEnum);
  bays = Object.values(BayEnum);

  form = this.fb.group({
    vinNumber: [{ value: '', disabled: true }, [Validators.required, Validators.pattern(VIN_REGEX)]],
    priority: [PriorityEnum.Medium, Validators.required],
    bay: [BayEnum.Bay1, Validators.required],
    dueDate: [''], // optional
  });

  constructor(
    private readonly fb: FormBuilder,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly carsStore: CarsStorage,
    private readonly toaster: ToasterService
  ) {}

  ngOnInit(): void {
    this.route.queryParamMap.subscribe(q => {
      const vin = (q.get('vin') ?? '').trim().toUpperCase();
      if (!vin) return;

      this.form.patchValue({ vinNumber: vin }, { emitEvent: false });

      // optional: verify car exists
      const car = this.carsStore.findByVin(vin);
      if (!car) {
        this.toaster.warn('Car not found for this VIN. Please create the car first.');
      }
    });
  }

  back(): void {
    this.router.navigateByUrl('/cars');
  }

  clear(): void {
    const vin = this.form.getRawValue().vinNumber;
    this.form.reset({
      vinNumber: vin,
      priority: PriorityEnum.Medium,
      bay: BayEnum.Bay1,
      dueDate: '',
    });
  }

  assign(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const v = this.form.getRawValue();

    // Here you can save assignment to local storage / backend later.
    this.toaster.success(`Assigned ${v.vinNumber} to ${v.bay} (${v.priority})`);
    this.back();
  }
}
