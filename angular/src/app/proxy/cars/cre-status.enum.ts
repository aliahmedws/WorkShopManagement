import { mapEnumToOptions } from '@abp/ng.core';

export enum CreStatus {
  Pending = 1,
  Submitted = 2,
}

export const creStatusOptions = mapEnumToOptions(CreStatus);
