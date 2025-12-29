import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';

import { LocalizationModule } from '@abp/ng.core';
import { ConfirmationService, ThemeSharedModule, ToasterService } from '@abp/ng.theme.shared';

import { Bay, Priority } from '../models/car-assignment.enums';
import { CarAssignmentsStorage } from '../models/car-assignment.storage';
import { WorkshopQueueStorage } from '../models/workshop-queue.storage';
import { CarsStorage } from '../models/cars.storage';
import { Car } from '../models/cars';

type OptionVm<T> = { value: T; label: string };

@Component({
  selector: 'app-assign-car-to-bay',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule, LocalizationModule, ThemeSharedModule],
  templateUrl: './assign-car-to-bay.html',
  styleUrls: ['./assign-car-to-bay.scss'],
})
export class AssignCarToBay implements OnInit {
  vinLocked = false;

  vinOptions: OptionVm<string>[] = [];
  private carByVin = new Map<string, Car>();

  readonly bayOptions: OptionVm<Bay>[] = [
    { value: Bay.Bay1, label: 'Bay #1' },
    { value: Bay.Bay2, label: 'Bay #2' },
    { value: Bay.Bay3, label: 'Bay #3' },
    { value: Bay.Bay4, label: 'Bay #4' },
    { value: Bay.Bay5, label: 'Bay #5' },
    { value: Bay.Bay6, label: 'Bay #6' },
    { value: Bay.Bay7, label: 'Bay #7' },
    { value: Bay.Bay8, label: 'Bay #8' },
  ];

  readonly priorityOptions: OptionVm<Priority>[] = [
    { value: Priority.Low, label: 'Low' },
    { value: Priority.Medium, label: 'Medium' },
    { value: Priority.High, label: 'High' },
    { value: Priority.Urgent, label: 'Urgent' },
  ];

  form = this.fb.group({
    // ✅ NOT disabled by default anymore (we will disable only when VIN is passed)
    vinNumber: this.fb.control<string>('', Validators.required),
    priority: this.fb.control<Priority | null>(null, Validators.required),
    bay: this.fb.control<Bay | null>(null, Validators.required),
    dueDate: this.fb.control<string | null>(null),
  });

  constructor(
    private readonly fb: FormBuilder,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly toaster: ToasterService,
    private readonly confirmation: ConfirmationService,
    private readonly assignmentsStore: CarAssignmentsStorage,
    private readonly queueStore: WorkshopQueueStorage,
    private readonly carsStore: CarsStorage
  ) {}

  ngOnInit(): void {
    const vinFromRoute =
      this.route.snapshot.queryParamMap.get('vin') ??
      (history.state?.vin as string | undefined);

    const normalized = this.normalizeVin(vinFromRoute);

    // Always load available VINs for the dropdown (used when VIN not provided)
    this.loadVinOptions();

    if (normalized) {
      // ✅ VIN provided: lock it
      this.vinLocked = true;
      this.form.patchValue({ vinNumber: normalized }, { emitEvent: false });
      this.form.get('vinNumber')?.disable({ emitEvent: false });
      return;
    }

    // ✅ VIN not provided: keep page open and user will select VIN
    this.vinLocked = false;
    this.form.get('vinNumber')?.enable({ emitEvent: false });
  }

  back(): void {
    this.router.navigateByUrl('/workshop');
  }

  clear(): void {
    const vin = this.vinLocked ? this.form.getRawValue().vinNumber : '';
    this.form.reset(
      { vinNumber: vin, priority: null, bay: null, dueDate: null },
      { emitEvent: false }
    );

    if (this.vinLocked) {
      this.form.get('vinNumber')?.disable({ emitEvent: false });
    } else {
      this.form.get('vinNumber')?.enable({ emitEvent: false });
    }
  }

  assign(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    const vin = this.normalizeVin(raw.vinNumber);

    if (!vin) {
      this.toaster.warn('VIN is required.');
      return;
    }

    // ✅ Validate VIN exists in Cars storage
    if (!this.carByVin.has(vin)) {
      this.toaster.warn('Selected VIN was not found in Cars.');
      return;
    }

    const bay = raw.bay!;
    const priority = raw.priority!;
    const dueDate = raw.dueDate ?? undefined;

    const latestByBay = this.assignmentsStore.getLatestByBay();
    const existingInBay = latestByBay.get(bay);

    const proceed = () => {
      // remove this car old assignment(s)
      this.assignmentsStore.removeAllByVin(vin);

      // if bay has another car -> unassign it and send back to queue
      if (existingInBay && this.normalizeVin(existingInBay.vinNumber) !== vin) {
        const oldVin = this.normalizeVin(existingInBay.vinNumber);
        this.assignmentsStore.removeAllByVin(oldVin);
        this.queueStore.addVin(oldVin);
      }

      // create new assignment
      this.assignmentsStore.create({
        vinNumber: vin,
        bay,
        priority,
        dueDate,
      });

      // remove assigned car from waiting list (if it was there)
      this.queueStore.removeVin(vin);

      this.toaster.success(`Assigned ${vin} to Bay #${bay}.`);
      this.router.navigateByUrl('/workshop');
    };

    if (existingInBay && this.normalizeVin(existingInBay.vinNumber) !== vin) {
      this.confirmation
        .warn(
          `Bay #${bay} is currently assigned to ${existingInBay.vinNumber}. Replace it?`,
          'Confirm'
        )
        .subscribe(status => {
          if (status === 'confirm') proceed();
        });
      return;
    }

    proceed();
  }

  // -----------------

  private loadVinOptions(): void {
    // Build car map (de-dupe by VIN, keep latest record)
    const cars = this.carsStore.getAll();
    this.carByVin = this.buildLatestCarByVin(cars);

    // Only allow cars not currently assigned to a bay
    const latestByBay = this.assignmentsStore.getLatestByBay();
    const assignedVins = new Set(
      Array.from(latestByBay.values()).map(a => this.normalizeVin(a.vinNumber))
    );

    // Prefer waiting list if it has values, otherwise show all unassigned cars
    const waiting = this.queueStore.getAllVins().map(v => this.normalizeVin(v));
    const waitingSet = new Set(waiting);

    const candidates: Car[] = [];
    for (const [vin, car] of this.carByVin.entries()) {
      if (assignedVins.has(vin)) continue;
      if (waitingSet.size > 0 && !waitingSet.has(vin)) continue;
      candidates.push(car);
    }

    this.vinOptions = candidates
      .sort((a, b) => (a.vinNumber ?? '').localeCompare(b.vinNumber ?? ''))
      .map(c => ({
        value: this.normalizeVin(c.vinNumber),
        label: `${this.normalizeVin(c.vinNumber)} • ${c.ownerName ?? '-'} • ${c.buildYear ?? '-'}`,
      }));
  }

  private buildLatestCarByVin(cars: Car[]): Map<string, Car> {
    const map = new Map<string, Car>();

    for (const c of cars) {
      const vin = this.normalizeVin(c.vinNumber);
      if (!vin) continue;

      const existing = map.get(vin);
      if (!existing) {
        map.set(vin, c);
        continue;
      }

      const a = (c.updatedAt ?? c.createdAt) ?? '';
      const b = (existing.updatedAt ?? existing.createdAt) ?? '';
      if (a.localeCompare(b) > 0) map.set(vin, c);
    }

    return map;
  }

  private normalizeVin(v?: string | null): string {
    return (v ?? '').trim().toUpperCase();
  }
}
