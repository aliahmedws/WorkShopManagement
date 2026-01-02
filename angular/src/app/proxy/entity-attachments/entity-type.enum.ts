import { mapEnumToOptions } from '@abp/ng.core';

export enum EntityType {
  CarModel = 1,
  CheckList = 2,
  ListItem = 3,
}

export const entityTypeOptions = mapEnumToOptions(EntityType);
