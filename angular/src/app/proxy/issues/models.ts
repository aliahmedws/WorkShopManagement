import type { IssueType } from './issue-type.enum';
import type { IssueStatus } from './issue-status.enum';
import type { IssueOriginStage } from './issue-origin-stage.enum';
import type { IssueDeteriorationType } from './issue-deterioration-type.enum';

export interface UpsertIssueDto {
  id?: string;
  xPercent: number;
  yPercent: number;
  type?: IssueType;
  status?: IssueStatus;
  originStage?: IssueOriginStage;
  deteriorationType?: IssueDeteriorationType;
  description: string;
  rectificationAction?: string;
}

export interface UpsertIssuesRequestDto {
  items: UpsertIssueDto[];
}
