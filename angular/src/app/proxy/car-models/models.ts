import type { FileAttachmentDto } from '../entity-attachments/file-attachments/models';
import type { PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CarModelDto {
  id?: string;
  modelCategoryId?: string;
  name?: string;
  fileAttachments: FileAttachmentDto;
}

export interface GetCarModelListDto extends PagedAndSortedResultRequestDto {
  filters?: string;
  modelCategoryId?: string;
  carModelId?: string;
}
