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

/** Dictionary record shape */
type VinDictionaryRecord = {
  vinNumber: string;          // key
  color: string;
  ownerContact?: string;
  description?: string;
  ownerName?: string;
  ownerEmail?: string;
  buildYear: string;
  carModelName: string;       // we'll convert name -> carModelId
};

/** ✅ 5–6 VIN records (STATIC) */
const VIN_DICTIONARY: Record<string, VinDictionaryRecord> = {
  // keys should be UPPERCASE VIN
  '1HGCM82633A004352': {
    vinNumber: '1HGCM82633A004352',
    color: 'White',
    ownerContact: '1001',
    description: 'From VIN dictionary',
    ownerName: 'Ali',
    ownerEmail: 'ali@example.com',
    buildYear: '2023',
    carModelName: 'Challenger Demon',
  },
  'JHMCM56557C404453': {
    vinNumber: 'JHMCM56557C404453',
    color: 'Black',
    ownerContact: '1002',
    description: 'From VIN dictionary',
    ownerName: 'Hassan',
    ownerEmail: 'hassan@example.com',
    buildYear: '2022',
    carModelName: 'Ram 1500',
  },
  '2T1BR32E54C331234': {
    vinNumber: '2T1BR32E54C331234',
    color: 'Silver',
    ownerContact: '1003',
    description: 'From VIN dictionary',
    ownerName: 'Usman',
    ownerEmail: 'usman@example.com',
    buildYear: '2021',
    carModelName: 'Titan',
  },
    '2T1BR32E54C331235': {
    vinNumber: '2T1BR32E54C331235',
    color: 'Blue',
    ownerContact: '1007',
    description: 'From VIN dictionary',
    ownerName: 'Ali',
    ownerEmail: 'ali@example.com',
    buildYear: '2021',
    carModelName: 'Challenger Charger',
  },
   '2T1BR32E54C331236': {
    vinNumber: '2T1BR32E54C331236',
    color: 'White',
    ownerContact: '10032342',
    description: 'From VIN dictionary',
    ownerName: 'ismail',
    ownerEmail: 'ismail@example.com',
    buildYear: '2021',
    carModelName: 'Ram 2500',
  },
};

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
      Validators.pattern(/^[A-HJ-NPR-Z0-9]{17}$/i),
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
      Validators.pattern(/^(19|20)\d{2}$/),
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
    // ensure car models exist
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
      // default model
      this.form.patchValue({ carModelId: this.makes[0].id }, { emitEvent: false });
    }
  }

 getDataByVin(): void {
  if (this.isEdit) return; // only allow in create mode (optional)

  const vinCtrl = this.form.get('vinNumber');
  vinCtrl?.markAsTouched();

  const vin = String(vinCtrl?.value ?? '').trim().toUpperCase();

  // Validate first (pattern validator already exists, so just rely on it)
  if (!vin || vinCtrl?.invalid) {
    this.toaster.warn('Please enter a valid 17-character VIN number.');
    return;
  }

  // 1) check dictionary
  const dictItem = VIN_DICTIONARY[vin];
  if (dictItem) {
    this.fillFromDictionary(dictItem);
    this.toaster.success('Data loaded from VIN dictionary.');
    return;
  }

  // 2) check existing cars in storage
  const existing = this.carsStore.findByVin(vin);
  if (existing) {
    this.fillFromExistingCar(existing);
    this.toaster.success('Data loaded from existing saved cars.');
    return;
  }

  // not found
  this.toaster.warn('No record found for this VIN.');
}


  private fillFromDictionary(d: VinDictionaryRecord): void {
    const carModelId = this.resolveCarModelIdByName(d.carModelName);

    this.form.patchValue(
      {
        color: d.color ?? '',
        ownerContact: d.ownerContact ?? '',
        description: d.description ?? '',
        ownerName: d.ownerName ?? '',
        ownerEmail: d.ownerEmail ?? '',
        buildYear: d.buildYear ?? '',
        carModelId: carModelId,
      },
      { emitEvent: false }
    );
  }

  private fillFromExistingCar(car: Car): void {
    this.form.patchValue(
      {
        color: car.color ?? '',
        ownerContact: car.ownerContact ?? '',
        description: car.description ?? '',
        ownerName: car.ownerName ?? '',
        ownerEmail: car.ownerEmail ?? '',
        buildYear: car.buildYear ?? '',
        carModelId: car.carModelId ?? this.makes[0].id,
      },
      { emitEvent: false }
    );
  }

  private resolveCarModelIdByName(name: string): string {
    const n = (name ?? '').trim().toLowerCase();
    const match = this.makes.find(m => (m.name ?? '').trim().toLowerCase() === n);
    return match?.id ?? this.makes[0].id;
  }

  back(): void {
    this.router.navigateByUrl('/cars');
  }

  clear(): void {
    this.form.reset();
    if (this.makes.length) {
      this.form.patchValue({ carModelId: this.makes[0].id }, { emitEvent: false });
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
    vinNumber: (v.vinNumber ?? '').trim().toUpperCase(),
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
      this.back();
      return;
    }

    const created = this.carsStore.create(payload);
    this.toaster.success('Created successfully');

    this.router.navigate(['/cars/assign-bay'], {
      queryParams: { vin: created.vinNumber },
    });

  } catch (e: any) {
    this.toaster.error(e?.message ?? 'Operation failed');
  }
}


  get vinCtrl() {
  return this.form.get('vinNumber');
}

get vinInvalid(): boolean {
  return !!this.vinCtrl && this.vinCtrl.touched && this.vinCtrl.invalid;
}

  
}
