import { mapEnumToOptions } from '@abp/ng.core';

export enum IssueStatus {
  Pending = 1,
  Deferred = 2,
  InProgress = 3,
  NoActionRequired = 4,
  Complete = 5,
}

export const issueStatusOptions = mapEnumToOptions(IssueStatus);
