import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { EntityAttachmentDto } from '../entity-attachments/models';
import type { ListItemDto } from '../list-items/models';
import type { CheckListProgressStatus } from './check-list-progress-status.enum';
import type { FileAttachmentDto } from '../entity-attachments/file-attachments/models';

export interface CheckListDto extends FullAuditedEntityDto<string> {
  name?: string;
  position: number;
  carModelId?: string;
  enableIssueItems?: boolean;
  enableTags?: boolean;
  enableCheckInReport?: boolean;
  concurrencyStamp?: string;
  entityAttachments: EntityAttachmentDto[];
  listItems: ListItemDto[];
  progressStatus?: CheckListProgressStatus;
}

export interface CreateCheckListDto {
  name: string;
  position: number;
  carModelId: string;
  enableIssueItems?: boolean;
  enableTags?: boolean;
  enableCheckInReport?: boolean;
  tempFiles: FileAttachmentDto[];
}

export interface GetCheckListListDto extends PagedAndSortedResultRequestDto {
  filter?: string;
  name?: string;
  position?: number;
  carModelId?: string;
}

export interface UpdateCheckListDto {
  name: string;
  position: number;
  carModelId: string;
  enableIssueItems?: boolean;
  enableTags?: boolean;
  enableCheckInReport?: boolean;
  concurrencyStamp: string;
  tempFiles: FileAttachmentDto[];
  entityAttachments: EntityAttachmentDto[];
}
