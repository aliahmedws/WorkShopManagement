import type { RecallType } from './recall-type.enum';
import type { RecallStatus } from './recall-status.enum';
import type { FileAttachmentDto } from '../entity-attachments/file-attachments/models';
import type { EntityAttachmentDto } from '../entity-attachments/models';
import type { EntityDto } from '@abp/ng.core';

export interface CreateRecallDto {
  carId: string;
  title: string;
  make?: string;
  manufactureId?: string;
  riskDescription?: string;
  type?: RecallType;
  status?: RecallStatus;
  notes?: string;
  tempFiles: FileAttachmentDto[];
}

export interface RecallDto {
  id?: string;
  title?: string;
  make?: string;
  manufactureId?: string;
  riskDescription?: string;
  type?: RecallType;
  status?: RecallStatus;
  notes?: string;
  isExternal: boolean;
  carId?: string;
  tempFiles: FileAttachmentDto[];
  entityAttachments: EntityAttachmentDto[];
  concurrencyStamp?: string;
}

export interface UpdateRecallDto extends EntityDto<string> {
  title: string;
  type: RecallType;
  status: RecallStatus;
  notes?: string;
  concurrencyStamp: string;
  tempFiles: FileAttachmentDto[];
  entityAttachments: EntityAttachmentDto[];
}
