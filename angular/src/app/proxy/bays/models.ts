import type { PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface BayDto {
  id?: string;
  name?: string;
  isActive: boolean;
}

export interface GetBayListInput extends PagedAndSortedResultRequestDto {
  filter?: string;
}
