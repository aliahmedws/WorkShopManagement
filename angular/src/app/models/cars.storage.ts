import { Injectable } from '@angular/core';
import { Car } from './cars';

@Injectable({ providedIn: 'root' })
export class CarsStorage {
  private readonly storageKey = 'app.cars.v1';

  getAll(): Car[] {
    return this.read().sort((a, b) => (b.updatedAt ?? b.createdAt).localeCompare(a.updatedAt ?? a.createdAt));
  }

  getById(id: string): Car | undefined {
    return this.read().find(x => x.id === id);
  }

  create(input: Omit<Car, 'id' | 'createdAt' | 'updatedAt'>): Car {
    const now = new Date().toISOString();
    const entity: Car = {
      id: this.newId(),
      ...input,
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

  private read(): Car[] {
    const raw = localStorage.getItem(this.storageKey);
    if (!raw) return [];
    try {
      const parsed = JSON.parse(raw) as Car[];
      return Array.isArray(parsed) ? parsed : [];
    } catch {
      return [];
    }
  }

  private write(items: Car[]): void {
    localStorage.setItem(this.storageKey, JSON.stringify(items));
  }

  private newId(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
      const r = crypto.getRandomValues(new Uint8Array(1))[0] & 15;
      const v = c === 'x' ? r : (r & 0x3) | 0x8;
      return v.toString(16);
    });
  }

  seedIfEmpty(defaults: Omit<Car, 'id' | 'createdAt' | 'updatedAt'>[]): void {
  const existing = this.read();
  if (existing.length > 0) return;

  for (const d of defaults) {
    this.create(d);
  }
}

}
