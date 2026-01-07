import type { PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { FileAttachmentDto } from '../entity-attachments/file-attachments/models';

export interface GetModelCategoryListDto extends PagedAndSortedResultRequestDto {
  filter?: string;
}

export interface ModelCategoryDto {
  id?: string;
  name?: string;
  fileAttachments: FileAttachmentDto;
}
