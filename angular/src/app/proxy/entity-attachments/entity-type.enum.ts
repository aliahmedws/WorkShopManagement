import { mapEnumToOptions } from '@abp/ng.core';

export enum EntityType {
  CarModel = 1,
  CheckList = 2,
  ListItem = 3,
  Car = 4,
  Recall = 5,
  CarBayItem = 6,
  Issue = 7,
  LogisticsDetail = 8,
  ArrivalEstimate = 9,
}

export const entityTypeOptions = mapEnumToOptions(EntityType);
