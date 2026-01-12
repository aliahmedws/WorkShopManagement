import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { IssueType } from './issue-type.enum';
import type { IssueStatus } from './issue-status.enum';
import type { Stage } from '../cars/stages/stage.enum';
import type { IssueOriginStage } from './issue-origin-stage.enum';
import type { IssueDeteriorationType } from './issue-deterioration-type.enum';
import type { EntityAttachmentDto } from '../entity-attachments/models';
import type { FileAttachmentDto } from '../entity-attachments/file-attachments/models';

export interface GetIssueListInput extends PagedAndSortedResultRequestDto {
  filter?: string;
  type?: IssueType;
  status?: IssueStatus;
  stage?: Stage;
}

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

export interface IssueListDto {
  id?: string;
  carId?: string;
  vin?: string;
  srNo: number;
  description?: string;
  type?: IssueType;
  status?: IssueStatus;
  stage?: Stage;
  creatorId?: string;
  creatorName?: string;
  creationTime?: string;
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
