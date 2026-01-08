import { mapEnumToOptions } from '@abp/ng.core';

export enum StorageLocation {
  K2 = 1,
  MoviesLoan = 2,
  ThirdPartyLoan = 3,
  OtherLoan = 4,
  EW = 5,
  TerrenceRd = 6,
}

export const storageLocationOptions = mapEnumToOptions(StorageLocation);
