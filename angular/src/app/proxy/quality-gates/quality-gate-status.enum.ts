import { mapEnumToOptions } from '@abp/ng.core';

export enum QualityGateStatus {
  PASSED = 1,
  CONDITIONALPASSEDMAJOR = 2,
  CONDITIONALPASSEDMINOR = 3,
  OPEN = 4,
  RESET = 5,
}

export const qualityGateStatusOptions = mapEnumToOptions(QualityGateStatus);
