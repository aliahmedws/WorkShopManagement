import { mapEnumToOptions } from '@abp/ng.core';

export enum StorageLocation {
  K2 = 1,
  TerrenceRoad = 2,
  MoviesLoan = 3,
  ThirdPartyLoan = 4,
  OtherLoan = 5,
  EW = 6,
}

export const storageLocationOptions = mapEnumToOptions(StorageLocation);
