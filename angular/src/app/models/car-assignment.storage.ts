import { Injectable } from '@angular/core';
import { CarAssignment } from './car-assignment';
import { Bay } from './car-assignment.enums';

@Injectable({ providedIn: 'root' })
export class CarAssignmentsStorage {
  private readonly storageKey = 'app.carAssignments.v1';

  getAll(): CarAssignment[] {
    return this.read().sort((a, b) =>
      (b.updatedAt ?? b.createdAt).localeCompare(a.updatedAt ?? a.createdAt)
    );
  }

  /** Latest assignment per bay (based on updatedAt/createdAt descending) */
  getLatestByBay(): Map<Bay, CarAssignment> {
    const map = new Map<Bay, CarAssignment>();
    for (const a of this.getAll()) {
      if (!map.has(a.bay)) map.set(a.bay, a);
    }
    return map;
  }

  getLatestForBay(bay: Bay): CarAssignment | undefined {
    return this.getLatestByBay().get(bay);
  }

  removeAllByVin(vinNumber: string): void {
    const vin = this.normalizeVin(vinNumber);
    if (!vin) return;
    this.write(this.read().filter(x => this.normalizeVin(x.vinNumber) !== vin));
  }

  removeAllByBay(bay: Bay): void {
    this.write(this.read().filter(x => x.bay !== bay));
  }

  create(input: Omit<CarAssignment, 'id' | 'createdAt' | 'updatedAt'>): CarAssignment {
    const now = new Date().toISOString();
    const entity: CarAssignment = {
      id: this.newId(),
      ...input,
      vinNumber: this.normalizeVin(input.vinNumber),
      createdAt: now,
      updatedAt: now,
    };

    const items = this.read();
    items.push(entity);
    this.write(items);
    return entity;
  }

  update(id: string, input: Omit<CarAssignment, 'id' | 'createdAt' | 'updatedAt'>): CarAssignment {
    const items = this.read();
    const idx = items.findIndex(x => x.id === id);
    if (idx < 0) throw new Error('Assignment not found.');

    const updated: CarAssignment = {
      ...items[idx],
      ...input,
      vinNumber: this.normalizeVin(input.vinNumber),
      updatedAt: new Date().toISOString(),
    };

    items[idx] = updated;
    this.write(items);
    return updated;
  }

  delete(id: string): void {
    this.write(this.read().filter(x => x.id !== id));
  }

  clear(): void {
    localStorage.removeItem(this.storageKey);
  }

  // -----------------

  private read(): CarAssignment[] {
    const raw = localStorage.getItem(this.storageKey);
    if (!raw) return [];

    try {
      const parsed = JSON.parse(raw) as CarAssignment[];
      return Array.isArray(parsed) ? parsed : [];
    } catch {
      return [];
    }
  }

  private write(items: CarAssignment[]): void {
    localStorage.setItem(this.storageKey, JSON.stringify(items));
  }

  private normalizeVin(vinNumber: string): string {
    return (vinNumber ?? '').trim().toUpperCase();
  }

  private newId(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
      const r = crypto.getRandomValues(new Uint8Array(1))[0] & 15;
      const v = c === 'x' ? r : (r & 0x3) | 0x8;
      return v.toString(16);
    });
  }
}
