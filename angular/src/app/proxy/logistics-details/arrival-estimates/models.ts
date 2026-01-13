import type { FullAuditedEntityDto } from '@abp/ng.core';
import type { EntityAttachmentDto } from '../../entity-attachments/models';
import type { FileAttachmentDto } from '../../entity-attachments/file-attachments/models';

export interface ArrivalEstimateDto extends FullAuditedEntityDto<string> {
  logisticsDetailId?: string;
  etaPort?: string;
  etaScd?: string;
  notes?: string;
  entityAttachments: EntityAttachmentDto[];
}

export interface CreateArrivalEstimateDto {
  logisticsDetailId: string;
  etaPort: string;
  etaScd: string;
  notes?: string;
  tempFiles: FileAttachmentDto[];
}

export interface UpdateArrivalEstimateDto {
  notes: string;
  entityAttachments: EntityAttachmentDto[];
  tempFiles: FileAttachmentDto[];
}
