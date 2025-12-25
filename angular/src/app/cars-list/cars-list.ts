import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';

import { LocalizationModule } from '@abp/ng.core';
import { ThemeSharedModule, ConfirmationService, ToasterService } from '@abp/ng.theme.shared';

import { CarsStorage } from '../models/cars.storage';
import { Car } from '../models/cars';
import { CarModelMakesStorage } from '../models/car-model-makes.storage';
import { CarModelMake } from '../models/car-model-makes';

@Component({
  selector: 'app-cars-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    NgxDatatableModule,
    NgbDropdownModule,
    LocalizationModule,
    ThemeSharedModule,
  ],
  templateUrl: './cars-list.html',
  styleUrls: ['./cars-list.scss'],
})
export class CarsList implements OnInit {
  all: Car[] = [];
  view: Car[] = [];
  filter = '';

  makes: CarModelMake[] = [];

  constructor(
    private readonly router: Router,
    private readonly carsStore: CarsStorage,
    private readonly makesStore: CarModelMakesStorage,
    private readonly confirmation: ConfirmationService,
    private readonly toaster: ToasterService
  ) {}

  ngOnInit(): void {
  // 1) Ensure car models exist
  this.makesStore.ensureDefaults();
  this.makes = this.makesStore.getAll();

  if (!this.makes.length) {
    this.toaster.error('Car models are not available');
    return;
  }

  // 2) Build set of valid make IDs
  const validMakeIds = new Set(this.makes.map(m => m.id));

  // 3) If cars exist but their carModelId is invalid => clear cars (fix old records problem)
  const existingCars = this.carsStore.getAll();
  const hasInvalidModel = existingCars.some(c => !validMakeIds.has(c.carModelId));

  if (hasInvalidModel) {
    this.carsStore.clear(); // removes old cars that point to old model IDs
  }

  // 4) Pick IDs from current makes list (correct IDs)
  const challengerDemonId =
    this.makes.find(x => x.name.toLowerCase() === 'challenger demon')?.id ?? this.makes[0].id;

  const ram1500Id =
    this.makes.find(x => x.name.toLowerCase() === 'ram 1500')?.id ?? this.makes[0].id;

  const titanId =
    this.makes.find(x => x.name.toLowerCase() === 'titan')?.id ?? this.makes[0].id;

  // 5) Seed 3 cars only if cars are empty
  this.carsStore.seedIfEmpty([
    {
      color: 'White',
      vinNumber: '1HGCM82633A004352',
      ownerContact: '1001',
      description: 'Default seeded car',
      ownerName: 'Ali',
      ownerEmail: 'ali@example.com',
      buildYear: '2023',
      carModelId: challengerDemonId,
    },
    {
      color: 'Black',
      vinNumber: 'JHMCM56557C404453',
      ownerContact: '1002',
      description: 'Default seeded car',
      ownerName: 'Hassan',
      ownerEmail: 'hassan@example.com',
      buildYear: '2022',
      carModelId: ram1500Id,
    },
    {
      color: 'Silver',
      vinNumber: '2T1BR32E54C331234',
      ownerContact: '1003',
      description: 'Default seeded car',
      ownerName: 'Usman',
      ownerEmail: 'usman@example.com',
      buildYear: '2021',
      carModelId: titanId,
    },
  ]);

  this.reload();
}


  reload(): void {
    this.all = this.carsStore.getAll();
    this.applyFilter();
  }

  applyFilter(): void {
    const q = (this.filter ?? '').trim().toLowerCase();
    if (!q) {
      this.view = this.all;
      return;
    }

    this.view = this.all.filter(x =>
      (x.vinNumber ?? '').toLowerCase().includes(q) ||
      (x.ownerName ?? '').toLowerCase().includes(q) ||
      (this.makeName(x.carModelId) ?? '').toLowerCase().includes(q)
    );
  }

  makeName(makeId: string): string {
    return this.makes.find(m => m.id === makeId)?.name ?? '-';
  }

  goCreate(): void {
    this.router.navigateByUrl('/cars/new');
  }

  goEdit(id: string): void {
    this.router.navigate(['/cars', id, 'edit']);
  }

  delete(row: Car): void {
    this.confirmation.warn(`Delete this car?`, 'Confirm').subscribe(status => {
      if (status !== 'confirm') return;
      this.carsStore.delete(row.id);
      this.toaster.success('Deleted successfully');
      this.reload();
    });
  }
}

