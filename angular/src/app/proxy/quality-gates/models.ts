import type { GateName } from './gate-name.enum';
import type { QualityGateStatus } from './quality-gate-status.enum';
import type { FullAuditedEntityDto } from '@abp/ng.core';

export interface CreateQualityGateDto {
  gateName: GateName;
  status: QualityGateStatus;
  carBayId: string;
}

export interface QualityGateDto extends FullAuditedEntityDto<string> {
  gateName?: GateName;
  status?: QualityGateStatus;
  carBayId?: string;
  concurrencyStamp?: string;
}

export interface UpdateQualityGateDto {
  gateName: GateName;
  status: QualityGateStatus;
  concurrencyStamp: string;
  carBayId: string;
}
