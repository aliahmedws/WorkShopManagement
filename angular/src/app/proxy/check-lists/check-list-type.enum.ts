import { mapEnumToOptions } from '@abp/ng.core';

export enum CheckListType {
  Production = 1,
  SubProduction = 2,
  AncillaryTask = 3,
}

export const checkListTypeOptions = mapEnumToOptions(CheckListType);
