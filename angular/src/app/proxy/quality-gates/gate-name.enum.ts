import { mapEnumToOptions } from '@abp/ng.core';

export enum GateName {
  PreProduction = 1,
  QualityControl = 2,
  QualityRelease = 3,
  PreDelivery = 4,
}

export const gateNameOptions = mapEnumToOptions(GateName);
