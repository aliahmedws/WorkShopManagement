import { Injectable } from '@angular/core';
import { CarModelMake, CarModelMakeImage } from './car-model-makes';

type CarModelSeed = { name: string; imagePaths: string[] };

// ✅ Your real folder: assets/images/cars-model
const DEFAULT_CAR_MODELS: CarModelSeed[] = [
  { name: 'Challenger Charger', imagePaths: ['/assets/images/cars-model/challenger-charger.jpg'] },
  { name: 'Challenger Demon',   imagePaths: ['/assets/images/cars-model/challenger-demon.jpg'] },
  { name: 'Challenger Hellcat', imagePaths: ['/assets/images/cars-model/challenger-hellcat.jpg'] },
  { name: 'Ram 1500',           imagePaths: ['/assets/images/cars-model/ram-1500.jpg'] },
  { name: 'Ram 2500',           imagePaths: ['/assets/images/cars-model/ram-2500.jpg'] },
  { name: 'Titan',              imagePaths: ['/assets/images/cars-model/titan.jpg'] },
];

@Injectable({ providedIn: 'root' })
export class CarModelMakesStorage {
  private readonly storageKey = 'app.carModelMakes.v2';

  // ✅ IMPORTANT: version key must match storage key, otherwise seeding will skip
  private readonly seedVersionKey = 'app.carModelMakes.v2.seedVersion';

  // bump this when you change defaults
  private readonly currentSeedVersion = 1;

  getAll(): CarModelMake[] {
    return this.read().sort((a, b) => a.name.localeCompare(b.name));
  }

  getById(id: string): CarModelMake | undefined {
    return this.read().find(x => x.id === id);
  }

  create(name: string, images: CarModelMakeImage[] = []): CarModelMake {
    const trimmed = (name ?? '').trim();
    if (!trimmed) throw new Error('Name is required.');

    const items = this.read();
    if (items.some(x => x.name.toLowerCase() === trimmed.toLowerCase())) {
      throw new Error('A record with the same name already exists.');
    }

    const now = new Date().toISOString();
    const entity: CarModelMake = {
      id: this.newId(),
      name: trimmed,
      images: images ?? [],
      createdAt: now,
      updatedAt: now,
    };

    items.push(entity);
    this.write(items);
    return entity;
  }

  update(id: string, name: string, images: CarModelMakeImage[] = []): CarModelMake {
    const trimmed = (name ?? '').trim();
    if (!trimmed) throw new Error('Name is required.');

    const items = this.read();
    const idx = items.findIndex(x => x.id === id);
    if (idx < 0) throw new Error('Record not found.');

    if (items.some(x => x.id !== id && x.name.toLowerCase() === trimmed.toLowerCase())) {
      throw new Error('A record with the same name already exists.');
    }

    const updated: CarModelMake = {
      ...items[idx],
      name: trimmed,
      images: images ?? [],
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
    localStorage.removeItem(this.seedVersionKey);
  }

  /**
   * ✅ Seeds static models + ensures at least 1 image per model.
   * ✅ Seeds when:
   *   - storage is empty, OR
   *   - seed version changed
   */
  ensureDefaults(): void {
    const items = this.read();
    const storedVersion = Number(localStorage.getItem(this.seedVersionKey) ?? '0');

    const needsSeed = items.length === 0 || storedVersion < this.currentSeedVersion;
    if (!needsSeed) return;

    // reset
    localStorage.removeItem(this.storageKey);

    const now = new Date().toISOString();
    const seeded: CarModelMake[] = DEFAULT_CAR_MODELS.map(seed => ({
      id: this.newId(),
      name: seed.name,
      createdAt: now,
      updatedAt: now,
      images: seed.imagePaths.map(p => this.toStaticImage(p)),
    }));

    this.write(seeded);
    localStorage.setItem(this.seedVersionKey, String(this.currentSeedVersion));
  }

  // ---------------- helpers ----------------

  private toStaticImage(path: string): CarModelMakeImage {
    return {
      id: this.newId(),
      fileName: path.split('/').pop() ?? 'image.jpg',
      contentType: this.guessContentType(path),
      dataUrl: path, // ✅ store static path here
    };
  }

  private guessContentType(path: string): string {
    const p = (path ?? '').toLowerCase();
    if (p.endsWith('.png')) return 'image/png';
    if (p.endsWith('.webp')) return 'image/webp';
    if (p.endsWith('.gif')) return 'image/gif';
    return 'image/jpeg';
  }

  private read(): CarModelMake[] {
    const raw = localStorage.getItem(this.storageKey);
    if (!raw) return [];

    try {
      const parsed = JSON.parse(raw) as any[];
      if (!Array.isArray(parsed)) return [];

      return parsed.map(x => ({
        id: String(x.id ?? this.newId()),
        name: String(x.name ?? ''),
        createdAt: String(x.createdAt ?? new Date().toISOString()),
        updatedAt: x.updatedAt ? String(x.updatedAt) : undefined,
        images: Array.isArray(x.images)
          ? x.images.map((img: any) => ({
              id: String(img.id ?? this.newId()),
              fileName: String(img.fileName ?? 'image.jpg'),
              contentType: String(img.contentType ?? 'image/jpeg'),
              dataUrl: String(img.dataUrl ?? ''),
            }))
          : [],
      }));
    } catch {
      return [];
    }
  }

  private write(items: CarModelMake[]): void {
    localStorage.setItem(this.storageKey, JSON.stringify(items));
  }

  private newId(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
      const r = crypto.getRandomValues(new Uint8Array(1))[0] & 15;
      const v = c === 'x' ? r : (r & 0x3) | 0x8;
      return v.toString(16);
    });
  }
}
