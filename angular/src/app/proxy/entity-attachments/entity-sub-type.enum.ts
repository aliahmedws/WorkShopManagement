import { mapEnumToOptions } from '@abp/ng.core';

export enum EntitySubType {
  Issue_Details = 0,
  Issue_RectificationAction = 1,
  Issue_QualityControl = 2,
  Issue_RepairerDetails = 3,
  ListItem_AttachmentFromBay = 4,
}

export const entitySubTypeOptions = mapEnumToOptions(EntitySubType);
