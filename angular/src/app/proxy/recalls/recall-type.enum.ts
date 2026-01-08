import { mapEnumToOptions } from '@abp/ng.core';

export enum RecallType {
  Recalls = 1,
  Csps = 2,
}

export const recallTypeOptions = mapEnumToOptions(RecallType);
