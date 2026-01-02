import type { FullAuditedEntityDto } from '@abp/ng.core';
import type { EntityType } from './entity-type.enum';
import type { FileAttachmentDto } from './file-attachments/models';

export interface EntityAttachmentDto extends FullAuditedEntityDto<string> {
  entityId?: string;
  entityType?: EntityType;
  attachment: FileAttachmentDto;
}
