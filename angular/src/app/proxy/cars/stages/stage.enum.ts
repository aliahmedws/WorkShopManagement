import { mapEnumToOptions } from '@abp/ng.core';

export enum Stage {
  Incoming = 1,
  ExternalWarehouse = 2,
  ScdWarehouse = 3,
  Production = 4,
  PostProduction = 5,
  AwaitingTransport = 6,
  Dispatched = 7,
}

export const stageOptions = mapEnumToOptions(Stage);
