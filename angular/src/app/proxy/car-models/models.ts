import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { FileAttachmentDto } from '../file-attachments/models';

export interface CarModelDto extends FullAuditedEntityDto<string> {
  name?: string;
  description?: string;
  fileAttachments: FileAttachmentDto;
}

export interface GetCarModelListDto extends PagedAndSortedResultRequestDto {
  filter?: string;
  name?: string;
}

export interface UpdateCarModelDto {
  name: string;
  description?: string;
  concurrencyStamp: string;
}
