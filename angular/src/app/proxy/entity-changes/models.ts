import type { EntityDto } from '@abp/ng.core';
import type { EntityChangeType } from '../volo/abp/auditing/entity-change-type.enum';

export interface EntityChangeDto extends EntityDto<string> {
  auditLogId?: string;
  tenantId?: string;
  changeTime?: string;
  changeType?: EntityChangeType;
  entityTenantId?: string;
  entityId?: string;
  entityTypeFullName?: string;
  propertyChanges: EntityPropertyChangeDto[];
}

export interface EntityChangeWithUsernameDto {
  entityChange: EntityChangeDto;
  userName?: string;
}

export interface EntityPropertyChangeDto extends EntityDto<string> {
  tenantId?: string;
  entityChangeId?: string;
  newValue?: string;
  originalValue?: string;
  propertyName?: string;
  propertyTypeFullName?: string;
}
