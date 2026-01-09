import { mapEnumToOptions } from '@abp/ng.core';

export enum IssueType {
  ScratchOrDent = 1,
  UpholsteryOrTrim = 2,
  ExternalPlastics = 3,
  MissingOrBrokenPart = 4,
  Compliance = 5,
  DTC = 6,
  Functional = 7,
  Other = 8,
}

export const issueTypeOptions = mapEnumToOptions(IssueType);
