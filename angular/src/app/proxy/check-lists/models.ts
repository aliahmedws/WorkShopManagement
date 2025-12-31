import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { CheckListType } from './check-list-type.enum';

export interface CheckListDto extends FullAuditedEntityDto<string> {
  name?: string;
  position: number;
  carModelId?: string;
  checkListType?: CheckListType;
}

export interface CreateCheckListDto {
  name: string;
  position: number;
  carModelId: string;
  checkListType: CheckListType;
}

export interface GetCheckListListDto extends PagedAndSortedResultRequestDto {
  filter?: string;
  name?: string;
  position?: number;
  checkListType?: CheckListType;
  carModelId?: string;
}

export interface UpdateCheckListDto {
  name: string;
  position: number;
  carModelId: string;
  checkListType: CheckListType;
  concurrencyStamp?: string;
}
