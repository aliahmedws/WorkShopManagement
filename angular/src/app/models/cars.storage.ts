import { Injectable } from '@angular/core';
import { Car } from './cars';

@Injectable({ providedIn: 'root' })
export class CarsStorage {
  private readonly storageKey = 'app.cars.v1';

  getAll(): Car[] {
    return this.read().sort((a, b) =>
      (b.updatedAt ?? b.createdAt).localeCompare(a.updatedAt ?? a.createdAt)
    );
  }

  getById(id: string): Car | undefined {
    return this.read().find(x => x.id === id);
  }

  findByVin(vinNumber: string): Car | undefined {
    const vin = this.normalizeVin(vinNumber);
    if (!vin) return undefined;

    return this.read().find(x => this.normalizeVin(x.vinNumber) === vin);
  }

  /**
   * ✅ Returns only cars explicitly marked as waiting list.
   */
  getWaitingList(): Car[] {
    return this.read().filter(x => !!x.isInWaitingList);
  }

  /**
   * ✅ Mark/unmark a car as waiting list by VIN.
   */
  setWaitingListByVin(vinNumber: string, isInWaitingList: boolean): void {
    const vin = this.normalizeVin(vinNumber);
    if (!vin) return;

    const items = this.read();
    const idx = items.findIndex(x => this.normalizeVin(x.vinNumber) === vin);
    if (idx < 0) throw new Error('Car not found.');

    items[idx] = {
      ...items[idx],
      isInWaitingList,
      updatedAt: new Date().toISOString(),
    };

    this.write(items);
  }

  create(input: Omit<Car, 'id' | 'createdAt' | 'updatedAt'>): Car {
    const now = new Date().toISOString();

    const entity: Car = {
      id: this.newId(),
      ...input,
      // ✅ default if not provided
      isInWaitingList: !!(input as any).isInWaitingList,
      createdAt: now,
      updatedAt: now,
    };

    const items = this.read();
    items.push(entity);
    this.write(items);
    return entity;
  }

  update(id: string, input: Omit<Car, 'id' | 'createdAt' | 'updatedAt'>): Car {
    const items = this.read();
    const idx = items.findIndex(x => x.id === id);
    if (idx < 0) throw new Error('Car not found.');

    const updated: Car = {
      ...items[idx],
      ...input,
      // ✅ preserve if caller doesn't set it
      isInWaitingList:
        input.isInWaitingList === undefined ? items[idx].isInWaitingList : input.isInWaitingList,
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

  seedIfEmpty(defaults: Omit<Car, 'id' | 'createdAt' | 'updatedAt'>[]): void {
    const existing = this.read();
    if (existing.length > 0) return;

    for (const d of defaults) {
      this.create(d);
    }
  }

  // ----------------- internal -----------------

  private read(): Car[] {
    const raw = localStorage.getItem(this.storageKey);
    if (!raw) return [];

    try {
      const parsed = JSON.parse(raw) as any[];
      if (!Array.isArray(parsed)) return [];

      // ✅ map + normalize older stored objects that don't have isInWaitingList
      return parsed.map((x: any) => ({
        id: String(x.id ?? this.newId()),
        color: String(x.color ?? ''),
        vinNumber: String(x.vinNumber ?? ''),
        ownerContact: x.ownerContact ? String(x.ownerContact) : undefined,
        description: x.description ? String(x.description) : undefined,
        ownerName: x.ownerName ? String(x.ownerName) : undefined,
        ownerEmail: x.ownerEmail ? String(x.ownerEmail) : undefined,
        buildYear: String(x.buildYear ?? ''),
        carModelId: String(x.carModelId ?? ''),
        isInWaitingList: !!x.isInWaitingList, // ✅ default false
        createdAt: String(x.createdAt ?? new Date().toISOString()),
        updatedAt: x.updatedAt ? String(x.updatedAt) : undefined,
      })) as Car[];
    } catch {
      return [];
    }
  }

  private write(items: Car[]): void {
    localStorage.setItem(this.storageKey, JSON.stringify(items));
  }

  private normalizeVin(vin: string | undefined | null): string {
    return (vin ?? '').trim().toUpperCase();
  }

  private newId(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
      const r = crypto.getRandomValues(new Uint8Array(1))[0] & 15;
      const v = c === 'x' ? r : (r & 0x3) | 0x8;
      return v.toString(16);
    });
  }
}
