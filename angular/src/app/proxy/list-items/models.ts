import type { CommentType } from './comment-type.enum';
import type { FileAttachmentDto } from '../entity-attachments/file-attachments/models';
import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { EntityAttachmentDto } from '../entity-attachments/models';

export interface CreateListItemDto {
  checkListId: string;
  position: number;
  name: string;
  commentPlaceholder?: string;
  commentType?: CommentType;
  isAttachmentRequired?: boolean;
  isSeparator?: boolean;
  tempFiles: FileAttachmentDto[];
}

export interface GetListItemListDto extends PagedAndSortedResultRequestDto {
  checkListId?: string;
  filter?: string;
}

export interface ListItemDto extends FullAuditedEntityDto<string> {
  checkListId?: string;
  position: number;
  name?: string;
  commentPlaceholder?: string;
  commentType?: CommentType;
  isAttachmentRequired?: boolean;
  isSeparator?: boolean;
  concurrencyStamp?: string;
  entityAttachments: EntityAttachmentDto[];
}

export interface UpdateListItemDto {
  checkListId: string;
  position: number;
  name: string;
  commentPlaceholder?: string;
  commentType?: CommentType;
  isAttachmentRequired?: boolean;
  isSeparator?: boolean;
  concurrencyStamp: string;
  tempFiles: FileAttachmentDto[];
  entityAttachments: EntityAttachmentDto[];
}
