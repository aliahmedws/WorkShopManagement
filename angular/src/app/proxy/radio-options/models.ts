import type { EntityDto } from '@abp/ng.core';

export interface CreateRadioOptionDto {
  listItemId: string;
  name: string;
}

export interface GetRadioOptionListDto {
  listItemId: string;
}

export interface RadioOptionDto extends EntityDto<string> {
  listItemId?: string;
  name?: string;
}

export interface UpdateRadioOptionDto {
  listItemId: string;
  name: string;
  concurrencyStamp?: string;
}
