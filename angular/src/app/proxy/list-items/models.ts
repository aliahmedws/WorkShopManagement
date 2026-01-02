import type { CommentType } from './comment-type.enum';
import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CreateListItemDto {
  checkListId: string;
  position: number;
  name: string;
  commentPlaceholder?: string;
  commentType?: CommentType;
  isAttachmentRequired?: boolean;
  isSeparator?: boolean;
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
}

export interface UpdateListItemDto {
  checkListId: string;
  position: number;
  name: string;
  commentPlaceholder?: string;
  commentType?: CommentType;
  isAttachmentRequired?: boolean;
  isSeparator?: boolean;
  concurrencyStamp?: string;
}
