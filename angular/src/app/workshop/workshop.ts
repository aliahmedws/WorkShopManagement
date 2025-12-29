import { ConfirmationService, ThemeSharedModule, ToasterService } from '@abp/ng.theme.shared';
import { Component, HostListener, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { LocalizationModule } from '@abp/ng.core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { CarAssignment } from '../models/car-assignment';
import { Bay, Priority } from '../models/car-assignment.enums';
import { CarAssignmentsStorage } from '../models/car-assignment.storage';

import { CarModelMake } from '../models/car-model-makes';
import { CarModelMakesStorage } from '../models/car-model-makes.storage';

import { Car } from '../models/cars';
import { CarsStorage } from '../models/cars.storage';

import { WorkshopQueueStorage } from '../models/workshop-queue.storage';

type BayVm = {
  bay: Bay;
  assignment: CarAssignment | null;
  car: Car | null;
  make: CarModelMake | null;
};

@Component({
  selector: 'app-workshop',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, LocalizationModule, ThemeSharedModule],
  templateUrl: './workshop.html',
  styleUrl: './workshop.scss',
})
export class Workshop implements OnInit {
  

  filter = '';

  cars: Car[] = [];
  makes: CarModelMake[] = [];
  assignments: CarAssignment[] = [];

  queue: Car[] = [];
  queueView: Car[] = [];
  bays: BayVm[] = [];

  // Details modal
  detailBay: BayVm | null = null;

  
  readonly detailStages: string[] = [
    'Arrival',
    'Compliance',
    'Wash _ Pre Inspection',
    'Wrap',
    'Disassembly',
    'Firewall',
    'Electricals_Looms',
    'Heater Box',
    'Dash Build',
    'Dash Pre Install',
    'Assembly',
    'Quality Check',
    'Test Drive',
    'Detailing',
    'Accessories',
    'Delivery',
  ];

  readonly bayList: Bay[] = [
    Bay.Bay1, Bay.Bay2, Bay.Bay3, Bay.Bay4,
    Bay.Bay5, Bay.Bay6, Bay.Bay7, Bay.Bay8,
  ];

  constructor(
    private readonly router: Router,
    private readonly carsStore: CarsStorage,
    private readonly makesStore: CarModelMakesStorage,
    private readonly assignmentsStore: CarAssignmentsStorage,
    private readonly queueStore: WorkshopQueueStorage,
    private readonly confirmation: ConfirmationService,
    private readonly toaster: ToasterService
  ) {}

  ngOnInit(): void {
    this.makesStore.ensureDefaults();
    this.refresh();
  }

  refresh(): void {
    this.makes = this.makesStore.getAll();
    this.assignments = this.assignmentsStore.getAll();

    // keep ONE car per VIN (prevents duplicates showing in queue)
    const carByVin = this.buildLatestCarByVin(this.carsStore.getAll());
    this.cars = Array.from(carByVin.values());

    const latestByBay = this.assignmentsStore.getLatestByBay();

    this.bays = this.bayList.map(bay => {
      const assignment = latestByBay.get(bay) ?? null;
      const car = assignment ? carByVin.get(this.normalizeVin(assignment.vinNumber)) ?? null : null;
      const make = car?.carModelId ? this.makesStore.getById(car.carModelId) ?? null : null;
      return { bay, assignment, car, make };
    });

    this.buildQueue(carByVin, latestByBay);
    this.applyFilter();
  }

  private buildQueue(carByVin: Map<string, Car>, latestByBay: Map<Bay, CarAssignment>): void {
    // Only show cars explicitly added to waiting list
    const waitingVins = new Set(this.queueStore.getAllVins().map(v => this.normalizeVin(v)));

    // Exclude cars currently assigned
    const assignedVins = new Set<string>();
    for (const a of latestByBay.values()) assignedVins.add(this.normalizeVin(a.vinNumber));

    const result: Car[] = [];
    for (const [vin, car] of carByVin.entries()) {
      if (!waitingVins.has(vin)) continue;
      if (assignedVins.has(vin)) continue;
      result.push(car);
    }

    this.queue = result;
  }

  applyFilter(): void {
    const q = (this.filter ?? '').trim().toLowerCase();
    if (!q) {
      this.queueView = this.queue;
      return;
    }

    this.queueView = this.queue.filter(c => {
      const vin = (c.vinNumber ?? '').toLowerCase();
      const owner = (c.ownerName ?? '').toLowerCase();
      const email = (c.ownerEmail ?? '').toLowerCase();
      const model = (this.makeName(c.carModelId) ?? '').toLowerCase();
      return vin.includes(q) || owner.includes(q) || email.includes(q) || model.includes(q);
    });
  }

  clearFilter(): void {
    this.filter = '';
    this.applyFilter();
  }

  // -------- actions --------

  assignFromQueue(car: Car): void {
    const vin = this.normalizeVin(car?.vinNumber);
    if (!vin) {
      this.toaster.warn('VIN was not provided.');
      return;
    }
    this.router.navigate(['/cars/assign-bay'], { queryParams: { vin } });
  }

  editAssignment(bay: BayVm): void {
    const vin = this.normalizeVin(bay.assignment?.vinNumber);
    if (!vin) return;
    this.router.navigate(['/cars/assign-bay'], { queryParams: { vin } });
  }

  unassignBay(bay: Bay): void {
    const latest = this.assignmentsStore.getLatestForBay(bay);

    this.confirmation.warn(`Unassign Bay #${bay}?`, 'Confirm').subscribe(status => {
      if (status !== 'confirm') return;

      this.assignmentsStore.removeAllByBay(bay);

      // return to waiting list
      if (latest?.vinNumber) this.queueStore.addVin(latest.vinNumber);

      this.toaster.success(`Bay #${bay} unassigned`);
      this.refresh();
    });
  }

  // -------- Details modal --------

  openDetails(bay: BayVm): void {
    this.detailBay = bay;
  }

  closeDetails(): void {
    this.detailBay = null;
  }

  addBayCarToWaitingList(): void {
  const bay = this.detailBay?.bay;
  const vin = this.normalizeVin(this.detailBay?.assignment?.vinNumber);

  if (!bay || !vin) return;

  this.confirmation
    .warn(`Move this car to the waiting list and unassign Bay #${bay}?`, 'Confirm')
    .subscribe(status => {
      if (status !== 'confirm') return;

      // 1) Unassign bay (removes assignment)
      this.assignmentsStore.removeAllByBay(bay);

      // 2) Add to waiting list
      this.queueStore.addVin(vin);

      // 3) Close modal + refresh UI
      this.closeDetails();
      this.toaster.success('Moved to waiting list.');
      this.refresh();
    });
}


  removeCarFromBay(): void {
    const bay = this.detailBay?.bay;
    if (!bay) return;
    this.closeDetails();
    this.unassignBay(bay);
  }

  @HostListener('document:keydown.escape')
  onEsc(): void {
    if (this.detailBay) this.closeDetails();
  }

  // -------- display helpers --------

  makeName(makeId: string | undefined | null): string {
    if (!makeId) return '-';
    return this.makes.find(m => m.id === makeId)?.name ?? '-';
  }

  bayLabel(bay: Bay): string {
    return `Bay #${bay}`;
  }

  priorityLabel(p?: Priority | null): string {
    switch (p) {
      case Priority.Low: return 'Low';
      case Priority.Medium: return 'Medium';
      case Priority.High: return 'High';
      case Priority.Urgent: return 'Urgent';
      default: return '-';
    }
  }

  priorityPillClass(p?: Priority | null): string {
    switch (p) {
      case Priority.Urgent: return 'ws-pill ws-pill--urgent';
      case Priority.High: return 'ws-pill ws-pill--high';
      case Priority.Medium: return 'ws-pill ws-pill--medium';
      case Priority.Low: return 'ws-pill ws-pill--low';
      default: return 'ws-pill ws-pill--neutral';
    }
  }

  vinShort(vinNumber: string | undefined | null): string {
    const vin = this.normalizeVin(vinNumber);
    if (!vin) return '-';
    if (vin.length <= 12) return vin;
    return `${vin.slice(0, 7)}...${vin.slice(-4)}`;
  }

  thumbUrlFor(make: CarModelMake | null): string {
    const url = make?.images?.[0]?.dataUrl;
    return url && url.trim().length ? url : this.placeholderThumb();
  }

  isOverdue(a: CarAssignment | null): boolean {
    if (!a?.dueDate) return false;
    const due = new Date(a.dueDate);
    const now = new Date();
    const d1 = new Date(due.getFullYear(), due.getMonth(), due.getDate()).getTime();
    const d2 = new Date(now.getFullYear(), now.getMonth(), now.getDate()).getTime();
    return d1 < d2;
  }

  waitingDays(a: CarAssignment | null): number {
    if (!a?.createdAt) return 0;
    const start = new Date(a.createdAt);
    const now = new Date();
    const diff = now.getTime() - start.getTime();
    return Math.max(0, Math.floor(diff / (1000 * 60 * 60 * 24)));
  }

  get assignedCount(): number {
    return this.bays.filter(b => !!b.assignment).length;
  }

  get overdueCount(): number {
    return this.bays.filter(b => this.isOverdue(b.assignment)).length;
  }

  trackByCarId(_: number, car: Car): string {
    return car.id;
  }

  // -------- internal --------

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

  private normalizeVin(vinNumber: string | undefined | null): string {
    return (vinNumber ?? '').trim().toUpperCase();
  }

  private placeholderThumb(): string {
    const svg = encodeURIComponent(`
      <svg xmlns="http://www.w3.org/2000/svg" width="160" height="120">
        <rect width="100%" height="100%" fill="#f1f3f5"/>
        <text x="50%" y="50%" font-size="14" font-family="Arial" fill="#868e96"
          dominant-baseline="middle" text-anchor="middle">No Image</text>
      </svg>
    `);
    return `data:image/svg+xml,${svg}`;
  }


  //
  
}
