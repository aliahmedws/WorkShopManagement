import { mapEnumToOptions } from '@abp/ng.core';

export enum IssueOriginStage {
  Logistics = 1,
  Production = 2,
}

export const issueOriginStageOptions = mapEnumToOptions(IssueOriginStage);
