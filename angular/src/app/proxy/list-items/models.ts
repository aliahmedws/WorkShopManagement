import type { CommentType } from './comment-type.enum';
import type { TempFileDto } from '../temp-files/models';
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
  tempFiles: TempFileDto[];
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
  attachments: EntityAttachmentDto[];
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
  tempFiles: TempFileDto[];
  attachments: EntityAttachmentDto[];
}
