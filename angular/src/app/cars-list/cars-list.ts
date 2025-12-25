import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

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
    this.makesStore.ensureDefaults();
    this.makes = this.makesStore.getAll();

    if (!this.makes.length) {
      this.toaster.error('Car models are not available');
      return;
    }

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

    this.view = this.all.filter(x => {
      const modelName = (this.makeName(x.carModelId) ?? '').toLowerCase();

      return (
        (x.vinNumber ?? '').toLowerCase().includes(q) ||
        modelName.includes(q) ||
        (x.color ?? '').toLowerCase().includes(q) ||
        (x.buildYear ?? '').toLowerCase().includes(q) ||
        (x.ownerName ?? '').toLowerCase().includes(q) ||
        (x.ownerEmail ?? '').toLowerCase().includes(q) ||
        (x.ownerContact ?? '').toLowerCase().includes(q) ||
        (x.description ?? '').toLowerCase().includes(q)
      );
    });
  }

  clearFilter(): void {
    this.filter = '';
    this.applyFilter();
  }

  makeName(makeId: string | undefined | null): string {
    if (!makeId) return '-';
    return this.makes.find(m => m.id === makeId)?.name ?? '-';
  }

  badgeClass(color?: string | null): string {
    const c = (color ?? '').trim().toLowerCase();
    if (c === 'white') return 'cm-badge cm-badge--white';
    if (c === 'black') return 'cm-badge cm-badge--black';
    if (c === 'silver') return 'cm-badge cm-badge--silver';
    if (c === 'blue') return 'cm-badge cm-badge--blue';
    return 'cm-badge cm-badge--default';
  }

  get carsWithModelCount(): number {
    return (this.all ?? []).filter(x => !!x.carModelId && this.makeName(x.carModelId) !== '-').length;
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

  trackById(_: number, item: Car): string {
    return item.id;
  }
}
