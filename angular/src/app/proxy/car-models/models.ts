import type { PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CarModelDto {
  id?: string;
  name?: string;
}

export interface GetCarModelListDto extends PagedAndSortedResultRequestDto {
}
