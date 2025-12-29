import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class WorkshopQueueStorage {
  private readonly storageKey = 'app.workshopQueue.v1';

  getAllVins(): string[] {
    return this.read();
  }

  hasVin(vinNumber: string): boolean {
    const vin = this.normalizeVin(vinNumber);
    if (!vin) return false;
    return this.read().includes(vin);
  }

  addVin(vinNumber: string): void {
    const vin = this.normalizeVin(vinNumber);
    if (!vin) return;

    const vins = this.read();
    if (vins.includes(vin)) return;

    vins.unshift(vin);
    this.write(vins);
  }

  removeVin(vinNumber: string): void {
    const vin = this.normalizeVin(vinNumber);
    if (!vin) return;

    const vins = this.read().filter(x => x !== vin);
    this.write(vins);
  }

  clear(): void {
    localStorage.removeItem(this.storageKey);
  }

  // -----------------

  private read(): string[] {
    const raw = localStorage.getItem(this.storageKey);
    if (!raw) return [];

    try {
      const parsed = JSON.parse(raw) as string[];
      if (!Array.isArray(parsed)) return [];
      return parsed.map(x => this.normalizeVin(x)).filter(Boolean) as string[];
    } catch {
      return [];
    }
  }

  private write(vins: string[]): void {
    localStorage.setItem(this.storageKey, JSON.stringify(vins));
  }

  private normalizeVin(vinNumber: string): string {
    return (vinNumber ?? '').trim().toUpperCase();
  }
}
