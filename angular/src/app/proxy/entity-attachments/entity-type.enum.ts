import { mapEnumToOptions } from '@abp/ng.core';

export enum EntityType {
  CarModel = 1,
  CheckList = 2,
  ListItem = 3,
  Car = 4,
  Recall = 5,
  CarBayItem = 6,
}

export const entityTypeOptions = mapEnumToOptions(EntityType);
