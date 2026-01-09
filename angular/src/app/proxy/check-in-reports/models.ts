import type { EntityDto, FullAuditedEntityDto } from '@abp/ng.core';
import type { ChoiceOptions } from '../utils/enums/choice-options.enum';
import type { StorageLocation } from '../cars/storage-locations/storage-location.enum';

export interface CheckInReportDto extends FullAuditedEntityDto<string> {
  buildYear?: number;
  buildMonth?: number;
  avcStickerCut?: ChoiceOptions;
  avcStickerPrinted?: ChoiceOptions;
  compliancePlatePrinted?: ChoiceOptions;
  complianceDate?: string;
  entryKms?: number;
  engineNumber?: string;
  frontGwar?: number;
  rearGwar?: number;
  frontMoterNumber?: string;
  rearMotorNumber?: string;
  maxTowingCapacity: number;
  emission?: string;
  tyreLabel?: string;
  reportStatus?: string;
  concurrencyStamp?: string;
  creatorName?: string;
  carId?: string;
  storageLocation?: StorageLocation;
}

export interface CheckInReportFiltersDto {
  sorting?: string;
  skipCount: number;
  maxResultCount: number;
  filter?: string;
  buildYear?: number;
  complianceDateMin?: string;
  complianceDateMax?: string;
  entryKmsMin?: number;
  entryKmsMax?: number;
  avcStickerCut?: ChoiceOptions;
  compliancePlatePrinted?: ChoiceOptions;
  reportStatus?: string;
  creatorId?: string;
  vin?: string;
  model?: string;
  storageLocation?: StorageLocation;
}

export interface CreateCheckInReportDto {
  buildYear?: number;
  buildMonth?: number;
  avcStickerCut?: ChoiceOptions;
  avcStickerPrinted?: ChoiceOptions;
  compliancePlatePrinted?: ChoiceOptions;
  complianceDate?: string;
  entryKms?: number;
  engineNumber?: string;
  frontGwar?: number;
  rearGwar?: number;
  frontMoterNumber?: string;
  rearMotorNumber?: string;
  maxTowingCapacity?: number;
  emission?: string;
  tyreLabel?: string;
  reportStatus?: string;
  carId: string;
  storageLocation?: StorageLocation;
}

export interface UpdateCheckInReportDto extends EntityDto<string> {
  buildYear?: number;
  buildMonth?: number;
  avcStickerCut?: ChoiceOptions;
  avcStickerPrinted?: ChoiceOptions;
  compliancePlatePrinted?: ChoiceOptions;
  complianceDate?: string;
  entryKms?: number;
  engineNumber?: string;
  frontGwar?: number;
  rearGwar?: number;
  frontMoterNumber?: string;
  rearMotorNumber?: string;
  maxTowingCapacity?: number;
  emission?: string;
  tyreLabel?: string;
  reportStatus?: string;
  storageLocation?: StorageLocation;
  concurrencyStamp: string;
}
