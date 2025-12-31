import { mapEnumToOptions } from '@abp/ng.core';

export enum CommentType {
  String = 1,
  Date = 2,
  Number = 3,
}

export const commentTypeOptions = mapEnumToOptions(CommentType);
