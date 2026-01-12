import type { EntityDto } from '@abp/ng.core';
import type { EntityType } from './entity-type.enum';
import type { EntitySubType } from './entity-sub-type.enum';
import type { FileAttachmentDto } from './file-attachments/models';

export interface EntityAttachmentDto extends EntityDto<string> {
  entityId?: string;
  entityType?: EntityType;
  subType?: EntitySubType;
  attachment: FileAttachmentDto;
}
