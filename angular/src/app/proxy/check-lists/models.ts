import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { EntityAttachmentDto } from '../entity-attachments/models';
import type { TempFileDto } from '../temp-files/models';

export interface CheckListDto extends FullAuditedEntityDto<string> {
  name?: string;
  position: number;
  carModelId?: string;
  enableIssueItems?: boolean;
  enableTags?: boolean;
  enableCheckInReport?: boolean;
  concurrencyStamp?: string;
  attachments: EntityAttachmentDto[];
}

export interface CreateCheckListDto {
  name: string;
  position: number;
  carModelId: string;
  enableIssueItems?: boolean;
  enableTags?: boolean;
  enableCheckInReport?: boolean;
  tempFiles: TempFileDto[];
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
  tempFiles: TempFileDto[];
  attachments: EntityAttachmentDto[];
}
