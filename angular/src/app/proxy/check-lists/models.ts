import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CheckListDto extends FullAuditedEntityDto<string> {
  name?: string;
  position: number;
  carModelId?: string;
  enableIssueItems?: boolean;
  enableTags?: boolean;
  enableCheckInReport?: boolean;
}

export interface CreateCheckListDto {
  name: string;
  position: number;
  carModelId: string;
  enableIssueItems?: boolean;
  enableTags?: boolean;
  enableCheckInReport?: boolean;
}

export interface GetCheckListListDto extends PagedAndSortedResultRequestDto {
  filter?: string;
  name?: string;
  position?: number;
  carModelId?: string;
}

export interface UpdateCheckListDto {
  name: string;
  position: number;
  carModelId: string;
  enableIssueItems?: boolean;
  enableTags?: boolean;
  enableCheckInReport?: boolean;
  concurrencyStamp?: string;
}
