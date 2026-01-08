import { mapEnumToOptions } from '@abp/ng.core';

export enum RecallStatus {
  Pending = 1,
  Completed = 2,
}

export const recallStatusOptions = mapEnumToOptions(RecallStatus);
