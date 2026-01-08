import type { RecallType } from './recall-type.enum';
import type { RecallStatus } from './recall-status.enum';
import type { FileAttachmentDto } from '../entity-attachments/file-attachments/models';
import type { EntityDto, FullAuditedEntityDto } from '@abp/ng.core';
import type { EntityAttachmentDto } from '../entity-attachments/models';

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

export interface ExternalRecallDetailDto {
  title?: string;
  make?: string;
  manufacturerId?: string;
  riskDescription?: string;
}

export interface RecallDto extends FullAuditedEntityDto<string> {
  title?: string;
  make?: string;
  manufactureId?: string;
  riskDescription?: string;
  type?: RecallType;
  status?: RecallStatus;
  notes?: string;
  carId?: string;
  vin?: string;
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
