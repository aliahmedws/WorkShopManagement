import { mapEnumToOptions } from '@abp/ng.core';

export enum ClockInStatus {
  NotClockedIn = 1,
  ClockedIn = 2,
  ClockedOut = 3,
}

export const clockInStatusOptions = mapEnumToOptions(ClockInStatus);
