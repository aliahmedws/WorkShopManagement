// signed-url-cache.service.ts
import { Injectable } from '@angular/core';

type CacheEntry = { url: string; expiresAtMs: number };

@Injectable({ providedIn: 'root' })
export class SignedUrlCacheService {
  private map = new Map<string, CacheEntry>();

  getValid(key: string, skewSeconds = 30): string | null {
    const e = this.map.get(key);
    if (!e) return null;
    if (Date.now() + skewSeconds * 1000 >= e.expiresAtMs) return null;
    return e.url;
  }

  set(key: string, url: string, lifetimeSeconds: number): void {
    this.map.set(key, { url, expiresAtMs: Date.now() + lifetimeSeconds * 1000 });
  }

  invalidate(key: string): void {
    this.map.delete(key);
  }
}