import { mapEnumToOptions } from '@abp/ng.core';

export enum AvvStatus {
  Required = 1,
  Inspected = 2,
  Uploaded = 3,
  NA_Export = 4,
  NA_NRI = 5,
  NA_MVSA = 6,
}

export const avvStatusOptions = mapEnumToOptions(AvvStatus);
