import type { PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { Stage } from '../cars/stages/stage.enum';
import type { Priority } from '../car-bays/priority.enum';
import type { RecallStatus } from '../recalls/recall-status.enum';
import type { IssueStatus } from '../issues/issue-status.enum';
import type { ClockInStatus } from '../car-bays/clock-in-status.enum';
import type { StorageLocation } from '../cars/storage-locations/storage-location.enum';
import type { AvvStatus } from '../car-bays/avv-status.enum';
import type { Port } from '../cars/port.enum';
import type { CreStatus } from '../cars/cre-status.enum';

export interface GetStageInput extends PagedAndSortedResultRequestDto {
  stage?: Stage;
  filter?: string;
}

export interface StageBayDto {
  bayId?: string;
  bayName?: string;
  carBayId?: string;
  priority?: Priority;
  carId?: string;
  vin?: string;
  manufactureStartDate?: string;
  ownerName?: string;
  modelName?: string;
  imageUrl?: string;
  recallStatus?: RecallStatus;
  issueStatus?: IssueStatus;
  clockInTime?: string;
  clockOutTime?: string;
  clockInStatus?: ClockInStatus;
}

export interface StageDto {
  carId?: string;
  vin?: string;
  stage?: Stage;
  color?: string;
  storageLocation?: StorageLocation;
  modelId?: string;
  lastModificationTime?: string;
  avvStatus?: AvvStatus;
  estimatedRelease?: string;
  notes?: string;
  ownerName?: string;
  modelName?: string;
  port?: Port;
  bookingNumber?: string;
  clearingAgent?: string;
  creStatus?: CreStatus;
  etaScd?: string;
  recallStatus?: RecallStatus;
  issueStatus?: IssueStatus;
  priority?: Priority;
  carBayId?: string;
  bayId?: string;
}
