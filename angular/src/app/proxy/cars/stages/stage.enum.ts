import { mapEnumToOptions } from '@abp/ng.core';

export enum Stage {
  Incoming = 1,
  ExternalWarehouse = 2,
  Production = 3,
  PostProduction = 4,
  AwaitingTransport = 5,
  Dispatched = 6,
}

export const stageOptions = mapEnumToOptions(Stage);
