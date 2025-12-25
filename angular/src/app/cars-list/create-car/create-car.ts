import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';

import { LocalizationModule } from '@abp/ng.core';
import { ThemeSharedModule, ToasterService } from '@abp/ng.theme.shared';

import { CarsStorage } from '../../models/cars.storage';
import { Car } from '../../models/cars';
import { CarModelMake } from '../../models/car-model-makes';
import { CarModelMakesStorage } from '../../models/car-model-makes.storage';

@Component({
  selector: 'app-create-car',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    LocalizationModule,
    ThemeSharedModule,
  ],
  templateUrl: './create-car.html',
  styleUrls: ['./create-car.scss'],
})
export class CreateCar implements OnInit {
  id: string | null = null;
  isEdit = false;

  makes: CarModelMake[] = [];

  form = this.fb.group({
  color: ['', Validators.required],

  vinNumber: ['', [
    Validators.required,
    Validators.pattern(/^[A-HJ-NPR-Z0-9]{17}$/i), // simple VIN-like rule (adjust if you want strict VIN)
  ]],

  ownerContact: [''],
  description: [''],

  ownerName: [''],

  ownerEmail: ['', [
    Validators.required,
    Validators.email,
  ]],

  buildYear: ['', [
    Validators.required,
    Validators.pattern(/^(19|20)\d{2}$/), // 1900-2099
  ]],

  carModelId: ['', Validators.required],
});


  constructor(
    private readonly fb: FormBuilder,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly carsStore: CarsStorage,
    private readonly makesStore: CarModelMakesStorage,
    private readonly toaster: ToasterService
  ) {}

  ngOnInit(): void {
     this.makesStore.ensureDefaults();

    this.makes = this.makesStore.getAll();
    if (!this.makes.length) {
      this.toaster.error('Car models are not available');
      this.back();
      return;
    }

    this.id = this.route.snapshot.paramMap.get('id');
    this.isEdit = !!this.id;

    if (this.isEdit) {
      const car = this.carsStore.getById(this.id!);
      if (!car) {
        this.toaster.error('Car not found');
        this.back();
        return;
      }

      this.form.patchValue({
        color: car.color,
        vinNumber: car.vinNumber,
        ownerContact: car.ownerContact ?? '',
        description: car.description ?? '',
        ownerName: car.ownerName ?? '',
        ownerEmail: car.ownerEmail ?? '',
        buildYear: car.buildYear,
        carModelId: car.carModelId,
      });
    } else {
      // default make
      if (this.makes.length) {
        this.form.patchValue({ carModelId: this.makes[0].id });
      }
    }
  }

  back(): void {
    this.router.navigateByUrl('/cars');
  }

  clear(): void {
    this.form.reset();
    if (this.makes.length) {
      this.form.patchValue({ carModelId: this.makes[0].id });
    }
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const v = this.form.value;

    const payload: Omit<Car, 'id' | 'createdAt' | 'updatedAt'> = {
      color: (v.color ?? '').trim(),
      vinNumber: (v.vinNumber ?? '').trim(),
      ownerContact: (v.ownerContact ?? '').trim() || undefined,
      description: (v.description ?? '').trim() || undefined,
      ownerName: (v.ownerName ?? '').trim() || undefined,
      ownerEmail: (v.ownerEmail ?? '').trim() || undefined,
      buildYear: (v.buildYear ?? '').trim(),
      carModelId: (v.carModelId ?? '').trim(),
    };

    try {
      if (this.isEdit) {
        this.carsStore.update(this.id!, payload);
        this.toaster.success('Updated successfully');
      } else {
        this.carsStore.create(payload);
        this.toaster.success('Created successfully');
      }

      this.back();
    } catch (e: any) {
      this.toaster.error(e?.message ?? 'Operation failed');
    }
  }
}
