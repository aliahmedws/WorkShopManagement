import { mapEnumToOptions } from '@abp/ng.core';

export enum Port {
  Bne = 1,
  syd = 2,
  mel = 3,
  per = 4,
  other = 5,
}

export const portOptions = mapEnumToOptions(Port);
