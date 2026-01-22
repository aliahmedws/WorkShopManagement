import { StickerItem } from '../models/sticker-item';

export class StickerGeneratorUtil {
  private static readonly BaseUrl = 'https://portal.bosscap.com.au/sticker_generator';

  static buildUrl(items: StickerItem[]): string {
    const url = new URL(StickerGeneratorUtil.BaseUrl);

    items.forEach((x, i) => {
      url.searchParams.set(`sticker_type_${i}`, x.stickerType);
      url.searchParams.set(`vin_${i}`, x.vin);
      url.searchParams.set(`bay_${i}`, x.bay);
      url.searchParams.set(`type_${i}`, x.type);
      url.searchParams.set(`date_${i}`, x.date);
      url.searchParams.set(`model_${i}`, x.model);

      if (x.flags) url.searchParams.set(`flags_${i}`, x.flags.trim());
      if (x.colour) url.searchParams.set(`colour_${i}`, x.colour);
      if (x.name) url.searchParams.set(`name_${i}`, x.name);
    });

    return url.toString();
  }

  static openInNewTab(items: StickerItem[]): void {
    const url = StickerGeneratorUtil.buildUrl(items);
    window.open(url, '_blank', 'noopener,noreferrer');
  }

  static vinLast6(v?: string | null): string {
    if (!v) return '';
    return v.length > 6 ? v.slice(-6) : v;
  }

  static bayLabel(bayName?: string | null): string {
    if (!bayName) return '';
    const n = bayName.toString().replace(/^bay\s*/i, '').trim();
    return n ? `Bay ${n}` : '';
  }

  static formatDate(d: Date): string {
    // "Wed, 9 July 2025"  (matches your screenshot style)
    const weekday = d.toLocaleDateString('en-US', { weekday: 'short' });
    const day = d.toLocaleDateString('en-US', { day: 'numeric' });
    const month = d.toLocaleDateString('en-US', { month: 'long' });
    const year = d.toLocaleDateString('en-US', { year: 'numeric' });
    return `${weekday}, ${day} ${month} ${year}`;
  }

  static manufactureDateLabel(value?: Date | string | null): string {
    if (!value) return '';
    const d = value instanceof Date ? value : new Date(value);
    if (Number.isNaN(d.getTime())) return '';
    return StickerGeneratorUtil.formatDate(d);
  }
}
