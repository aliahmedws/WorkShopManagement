import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { EntityAttachmentDto } from '../entity-attachments/models';
import type { FileAttachmentDto } from '../entity-attachments/file-attachments/models';

export interface CarBayItemDto extends FullAuditedEntityDto<string> {
  checkListItemId?: string;
  carBayId?: string;
  checkRadioOption?: string;
  comments?: string;
  entityAttachments: EntityAttachmentDto[];
}

export interface CreateCarBayItemDto {
  checkListItemId: string;
  carBayId: string;
  checkRadioOption?: string;
  comments?: string;
  tempFiles: FileAttachmentDto[];
}

export interface GetCarBayItemListDto extends PagedAndSortedResultRequestDto {
  filter?: string;
  checkListItemId?: string;
  carBayId?: string;
}

export interface UpdateCarBayItemDto {
  checkListItemId: string;
  carBayId: string;
  checkRadioOption?: string;
  comments?: string;
  concurrencyStamp: string;
  tempFiles: FileAttachmentDto[];
  entityAttachments: EntityAttachmentDto[];
}
