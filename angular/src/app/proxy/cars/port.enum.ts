import { mapEnumToOptions } from '@abp/ng.core';

export enum Port {
  Bne = 1,
  Syd = 2,
  Mel = 3,
  Per = 4,
  Other = 5,
}

export const portOptions = mapEnumToOptions(Port);
