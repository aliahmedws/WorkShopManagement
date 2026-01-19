import type { EntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { Priority } from './priority.enum';
import type { AvvStatus } from './avv-status.enum';
import type { ClockInStatus } from './clock-in-status.enum';
import type { Port } from '../cars/port.enum';
import type { CheckListDto } from '../check-lists/models';

export interface CarBayDto extends EntityDto<string> {
  carId?: string;
  bayId?: string;
  priority?: Priority;
  buildMaterialNumber?: string;
  userId?: string;
  qualityGateId?: string;
  dateTimeIn?: string;
  dateTimeOut?: string;
  isActive?: boolean;
  isWaiting?: boolean;
  isQueue?: boolean;
  angleBailment?: number;
  avvStatus?: AvvStatus;
  pdiStatus?: string;
  displayBay?: number;
  percentage?: number;
  dueDate?: string;
  dueDateUpdated?: string;
  confirmedDeliverDate?: string;
  confirmedDeliverDateNotes?: string;
  transportDestination?: string;
  storageLocation?: string;
  row?: string;
  columns?: string;
  reWorkDate?: string;
  manufactureStartDate?: string;
  pulseNumber?: string;
  canProgress?: boolean;
  jobCardCompleted?: boolean;
  bayName?: string;
  bayNumber?: number;
  carVin?: string;
  ownerName?: string;
  modelName?: string;
  modelCategoryName?: string;
  modelImagePath?: string;
  clockInTime?: string;
  clockOutTime?: string;
  clockInStatus?: ClockInStatus;
  port?: Port;
  checkLists: CheckListDto[];
}

export interface CreateCarBayDto {
  carId: string;
  bayId: string;
  priority?: Priority;
  buildMaterialNumber?: string;
  userId?: string;
  qualityGateId?: string;
  dateTimeIn?: string;
  dateTimeOut?: string;
  isActive?: boolean;
  isWaiting?: boolean;
  isQueue?: boolean;
  angleBailment?: number;
  avvStatus?: AvvStatus;
  pdiStatus?: string;
  displayBay?: number;
  percentage?: number;
  dueDate?: string;
  dueDateUpdated?: string;
  confirmedDeliverDate?: string;
  confirmedDeliverDateNotes?: string;
  transportDestination?: string;
  storageLocation?: string;
  row?: string;
  columns?: string;
  reWorkDate?: string;
  manufactureStartDate?: string;
  pulseNumber?: string;
  canProgress?: boolean;
  jobCardCompleted?: boolean;
}

export interface GetCarBayListDto extends PagedAndSortedResultRequestDto {
  filter?: string;
  carId?: string;
  bayId?: string;
  isActive?: boolean;
}

export interface UpdateCarBayDto {
  carId: string;
  bayId: string;
  priority?: Priority;
  buildMaterialNumber?: string;
  userId?: string;
  qualityGateId?: string;
  dateTimeIn?: string;
  dateTimeOut?: string;
  isActive?: boolean;
  isWaiting?: boolean;
  isQueue?: boolean;
  angleBailment?: number;
  avvStatus?: AvvStatus;
  pdiStatus?: string;
  displayBay?: number;
  percentage?: number;
  dueDate?: string;
  dueDateUpdated?: string;
  confirmedDeliverDate?: string;
  confirmedDeliverDateNotes?: string;
  transportDestination?: string;
  storageLocation?: string;
  row?: string;
  columns?: string;
  reWorkDate?: string;
  manufactureStartDate?: string;
  pulseNumber?: string;
  canProgress?: boolean;
  jobCardCompleted?: boolean;
  concurrencyStamp: string;
}
