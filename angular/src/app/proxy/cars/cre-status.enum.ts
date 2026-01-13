import { mapEnumToOptions } from '@abp/ng.core';

export enum CreStatus {
  Pending = 1,
  Submitted = 3,
}

export const creStatusOptions = mapEnumToOptions(CreStatus);
