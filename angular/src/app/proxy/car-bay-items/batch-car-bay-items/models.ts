import type { CarBayItemDto } from '../models';
import type { FileAttachmentDto } from '../../entity-attachments/file-attachments/models';
import type { EntityAttachmentDto } from '../../entity-attachments/models';

export interface SaveCarBayItemBatchDto {
  carBayId?: string;
  items: SaveCarBayItemRowDto[];
}

export interface SaveCarBayItemBatchResultDto {
  items: CarBayItemDto[];
}

export interface SaveCarBayItemRowDto {
  id?: string;
  checkListItemId?: string;
  carBayId?: string;
  checkRadioOption?: string;
  comments?: string;
  tempFiles: FileAttachmentDto[];
  entityAttachments: EntityAttachmentDto[];
  concurrencyStamp?: string;
}
