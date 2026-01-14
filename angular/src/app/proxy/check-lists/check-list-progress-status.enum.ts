import { mapEnumToOptions } from '@abp/ng.core';

export enum CheckListProgressStatus {
  Pending = 0,
  InProgress = 1,
  Completed = 2,
}

export const checkListProgressStatusOptions = mapEnumToOptions(CheckListProgressStatus);
