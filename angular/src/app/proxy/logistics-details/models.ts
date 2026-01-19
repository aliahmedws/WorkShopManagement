import type { Port } from '../cars/port.enum';
import type { FileAttachmentDto } from '../entity-attachments/file-attachments/models';
import type { FullAuditedEntityDto } from '@abp/ng.core';
import type { CreStatus } from '../cars/cre-status.enum';
import type { ArrivalEstimateDto } from './arrival-estimates/models';
import type { EntityAttachmentDto } from '../entity-attachments/models';

export interface AddOrUpdateDeliverDetailDto {
  confirmedDeliverDate?: string;
  deliverNotes?: string;
  deliverTo?: string;
  transportDestination?: string;
}

export interface CreateLogisticsDetailDto {
  carId: string;
  port: Port;
  bookingNumber?: string;
  tempFiles: FileAttachmentDto[];
}

export interface LogisticsDetailDto extends FullAuditedEntityDto<string> {
  carId?: string;
  port?: Port;
  bookingNumber?: string;
  creStatus?: CreStatus;
  creSubmissionDate?: string;
  rsvaNumber?: string;
  clearingAgent?: string;
  clearanceRemarks?: string;
  clearanceDate?: string;
  actualPortArrivalDate?: string;
  actualScdArrivalDate?: string;
  deliverTo?: string;
  confirmedDeliverDate?: string;
  deliverNotes?: string;
  transportDestination?: string;
  arrivalEstimates: ArrivalEstimateDto[];
  entityAttachments: EntityAttachmentDto[];
}

export interface UpdateLogisticsDetailDto {
  port: Port;
  bookingNumber?: string;
  creStatus: CreStatus;
  creSubmissionDate?: string;
  rsvaNumber?: string;
  clearingAgent?: string;
  clearanceRemarks?: string;
  clearanceDate?: string;
  actualPortArrivalDate?: string;
  actualScdArrivalDate?: string;
  entityAttachments: EntityAttachmentDto[];
  tempFiles: FileAttachmentDto[];
}
