import type { EntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CreatePriorityDto {
  number: number;
  description?: string;
}

export interface GetPriorityListDto extends PagedAndSortedResultRequestDto {
  filter?: string;
}

export interface PriorityDto extends EntityDto<string> {
  number: number;
  description?: string;
}

export interface UpdatePriorityDto {
  number: number;
  description?: string;
}
