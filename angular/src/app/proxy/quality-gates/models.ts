import type { GateName } from './gate-name.enum';
import type { QualityGateStatus } from './quality-gate-status.enum';
import type { FullAuditedEntityDto } from '@abp/ng.core';

export interface CreateQualityGateDto {
  gateName: GateName;
  status: QualityGateStatus;
}

export interface QualityGateDto extends FullAuditedEntityDto<string> {
  gateName?: GateName;
  status?: QualityGateStatus;
}

export interface UpdateQualityGateDto {
  gateName: GateName;
  status: QualityGateStatus;
  concurrencyStamp: string;
}
