import type { FullAuditedEntityDto } from '@abp/ng.core';
import type { IssueType } from './issue-type.enum';
import type { IssueStatus } from './issue-status.enum';
import type { IssueOriginStage } from './issue-origin-stage.enum';
import type { IssueDeteriorationType } from './issue-deterioration-type.enum';
import type { EntityAttachmentDto } from '../entity-attachments/models';
import type { FileAttachmentDto } from '../entity-attachments/file-attachments/models';

export interface IssueDto extends FullAuditedEntityDto<string> {
  srNo: number;
  carId?: string;
  xPercent: number;
  yPercent: number;
  type?: IssueType;
  status?: IssueStatus;
  originStage?: IssueOriginStage;
  deteriorationType?: IssueDeteriorationType;
  description?: string;
  rectificationAction?: string;
  rectificationNotes?: string;
  qualityControlAction?: string;
  qualityControlNotes?: string;
  repairerAction?: string;
  repairerNotes?: string;
  creatorEmail?: string;
  lastModifierEmail?: string;
  entityAttachments: EntityAttachmentDto[];
}

export interface UpsertIssueDto {
  id?: string;
  srNo: number;
  xPercent: number;
  yPercent: number;
  type?: IssueType;
  status?: IssueStatus;
  originStage?: IssueOriginStage;
  deteriorationType?: IssueDeteriorationType;
  description: string;
  rectificationAction?: string;
  rectificationNotes?: string;
  qualityControlAction?: string;
  qualityControlNotes?: string;
  repairerAction?: string;
  repairerNotes?: string;
  tempFiles: FileAttachmentDto[];
  entityAttachments: EntityAttachmentDto[];
}

export interface UpsertIssuesRequestDto {
  items: UpsertIssueDto[];
}
