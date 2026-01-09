import type { CheckInReportDto, CheckInReportFiltersDto, CreateCheckInReportDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CheckInReportService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateCheckInReportDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CheckInReportDto>({
      method: 'POST',
      url: '/api/app/check-in-reports',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  get = (checkInReportId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CheckInReportDto>({
      method: 'GET',
      url: `/api/app/check-in-reports/${checkInReportId}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (filter: CheckInReportFiltersDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<CheckInReportDto>>({
      method: 'GET',
      url: '/api/app/check-in-reports',
      params: { sorting: filter.sorting, skipCount: filter.skipCount, maxResultCount: filter.maxResultCount, filter: filter.filter, vinNo: filter.vinNo, status: filter.status, model: filter.model, storageLocation: filter.storageLocation, buildDateMin: filter.buildDateMin, buildDateMax: filter.buildDateMax, complianceDateMin: filter.complianceDateMin, complianceDateMax: filter.complianceDateMax, entryKmsMin: filter.entryKmsMin, entryKmsMax: filter.entryKmsMax, avcStickerCut: filter.avcStickerCut, compliancePlatePrinted: filter.compliancePlatePrinted },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateCheckInReportDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CheckInReportDto>({
      method: 'PUT',
      url: '/api/app/check-in-reports',
      params: { id },
      body: input,
    },
    { apiName: this.apiName,...config });
}