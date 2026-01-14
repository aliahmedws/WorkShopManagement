import { VehicleStickerV2Item } from '../models/vehicle-sticker-v2';

export class VehicleStickerV2Util {
  private static readonly BaseUrl = 'https://portal.bosscap.com.au/vehicle_sticker_v2';

  static buildUrl(item: VehicleStickerV2Item): string {
    const url = new URL(VehicleStickerV2Util.BaseUrl);

    url.searchParams.set('vin', item.vin);
    url.searchParams.set('color', item.color);
    url.searchParams.set('dealer', item.dealer);
    url.searchParams.set('image', item.image); // will be encoded properly
    url.searchParams.set('model', item.model);
    url.searchParams.set('owner', item.owner);

    return url.toString();
  }

  static openInNewTab(item: VehicleStickerV2Item): void {
    const url = VehicleStickerV2Util.buildUrl(item);
    window.open(url, '_blank', 'noopener,noreferrer');
  }
}
