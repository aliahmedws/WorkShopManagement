import type { AuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { Stage } from './stages/stage.enum';
import type { StorageLocation } from './storage-locations/storage-location.enum';
import type { AvvStatus } from '../car-bays/avv-status.enum';
import type { EntityAttachmentDto } from '../entity-attachments/models';
import type { Port } from './port.enum';
import type { FileAttachmentDto } from '../entity-attachments/file-attachments/models';

export interface CarDto extends AuditedEntityDto<string> {
  modelId?: string;
  modelName?: string;
  ownerId?: string;
  ownerName?: string;
  ownerEmail?: string;
  ownerContactId?: string;
  vin?: string;
  color?: string;
  modelYear: number;
  stage?: Stage;
  cnc?: string;
  cncFirewall?: string;
  cncColumn?: string;
  dueDate?: string;
  deliverDate?: string;
  startDate?: string;
  dueDateUpdated?: string;
  notes?: string;
  missingParts?: string;
  storageLocation?: StorageLocation;
  buildMaterialNumber?: string;
  angleBailment?: number;
  avvStatus?: AvvStatus;
  pdiStatus?: string;
  bayId?: string;
  bayName?: string;
  entityAttachments: EntityAttachmentDto[];
}

export interface ChangeCarStageDto {
  targetStage?: Stage;
  storageLocation?: StorageLocation;
}

export interface CreateCarDto {
  ownerId?: string;
  owner: CreateCarOwnerDto;
  vin: string;
  color: string;
  modelId: string;
  modelYear: number;
  cnc?: string;
  cncFirewall?: string;
  cncColumn?: string;
  dueDate?: string;
  deliverDate?: string;
  startDate?: string;
  notes?: string;
  missingParts?: string;
  port?: Port;
  bookingNumber?: string;
  storageLocation?: StorageLocation;
  buildMaterialNumber?: string;
  angleBailment?: number;
  avvStatus?: AvvStatus;
  pdiStatus?: string;
  tempFiles: FileAttachmentDto[];
}

export interface CreateCarOwnerDto {
  name: string;
  email?: string;
  contactId?: string;
}

export interface ExternalCarDetailsDto {
  model?: string;
  modelYear?: string;
  suggestedVin?: string;
  error?: string;
  success: boolean;
}

export interface GetCarListInput extends PagedAndSortedResultRequestDto {
  filter?: string;
  stage?: Stage;
}

export interface UpdateCarDto {
  ownerId?: string;
  owner: CreateCarOwnerDto;
  vin: string;
  color: string;
  modelId: string;
  modelYear: number;
  stage: Stage;
  cnc?: string;
  cncFirewall?: string;
  cncColumn?: string;
  dueDate?: string;
  deliverDate?: string;
  startDate?: string;
  notes?: string;
  missingParts?: string;
  storageLocation?: StorageLocation;
  buildMaterialNumber?: string;
  angleBailment?: number;
  avvStatus?: AvvStatus;
  pdiStatus?: string;
  entityAttachments: EntityAttachmentDto[];
  tempFiles: FileAttachmentDto[];
}
