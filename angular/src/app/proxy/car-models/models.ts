import type { FileAttachmentDto } from '../file-attachments/models';
import type { PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CarModelDto {
  id?: string;
  name?: string;
  fileAttachments: FileAttachmentDto;
}

export interface GetCarModelListDto extends PagedAndSortedResultRequestDto {
}
