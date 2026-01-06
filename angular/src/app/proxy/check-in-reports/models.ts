import type { AuditedEntityDto } from '@abp/ng.core';

export interface CheckInReportDto extends AuditedEntityDto<string> {
  vinNo?: string;
  avcStickerCut?: boolean;
  avcStickerPrinted?: boolean;
  buildDate?: string;
  checkInSumbitterUser?: string;
  complianceDate?: string;
  compliancePlatePrinted?: boolean;
  emission?: string;
  engineNumber?: string;
  entryKms?: number;
  frontGwar?: number;
  frontMoterNumbr?: string;
  rearGwar?: number;
  rearMotorNumber?: string;
  hsObjectId?: string;
  maxTowingCapacity: number;
  tyreLabel?: string;
  rsvaImportApproval?: string;
  status?: string;
  model?: string;
  storageLocation?: string;
  carId?: string;
}

export interface CheckInReportFiltersDto {
  sorting?: string;
  skipCount: number;
  maxResultCount: number;
  filter?: string;
  vinNo?: string;
  status?: string;
  model?: string;
  storageLocation?: string;
  buildDateMin?: string;
  buildDateMax?: string;
  complianceDateMin?: string;
  complianceDateMax?: string;
  entryKmsMin?: number;
  entryKmsMax?: number;
  avcStickerCut?: boolean;
  compliancePlatePrinted?: boolean;
}

export interface CreateCheckInReportDto {
  vinNo?: string;
  avcStickerCut?: boolean;
  avcStickerPrinted?: boolean;
  buildDate?: string;
  checkInSumbitterUser?: string;
  complianceDate?: string;
  compliancePlatePrinted?: boolean;
  emission?: string;
  engineNumber?: string;
  entryKms?: number;
  frontGwar?: number;
  frontMoterNumbr?: string;
  rearGwar?: number;
  rearMotorNumber?: string;
  hsObjectId?: string;
  maxTowingCapacity: number;
  tyreLabel?: string;
  rsvaImportApproval?: string;
  status?: string;
  model?: string;
  storageLocation?: string;
  carId?: string;
}
