import type { AuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { Stage } from '../stages/stage.enum';
import type { StorageLocation } from '../storage-locations/storage-location.enum';

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
  locationStatus?: string;
  etaBrisbane?: string;
  etaScd?: string;
  bookingNumber?: string;
  clearingAgent?: string;
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
  locationStatus?: string;
  etaBrisbane?: string;
  etaScd?: string;
  bookingNumber?: string;
  clearingAgent?: string;
  storageLocation?: StorageLocation;
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
  locationStatus?: string;
  etaBrisbane?: string;
  etaScd?: string;
  bookingNumber?: string;
  clearingAgent?: string;
  storageLocation?: StorageLocation;
}
