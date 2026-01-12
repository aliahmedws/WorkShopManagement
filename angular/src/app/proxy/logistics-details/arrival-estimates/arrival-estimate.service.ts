import type { ArrivalEstimateDto, CreateArrivalEstimateDto, UpdateArrivalEstimateDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ArrivalEstimateService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateArrivalEstimateDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ArrivalEstimateDto>({
      method: 'POST',
      url: '/api/app/arrival-estimates',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/arrival-estimates/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ArrivalEstimateDto>({
      method: 'GET',
      url: `/api/app/arrival-estimates/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getLatest = (logisticsDetailId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ArrivalEstimateDto>({
      method: 'GET',
      url: `/api/app/arrival-estimates/latest/${logisticsDetailId}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (logisticsDetailId: string, input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ArrivalEstimateDto>>({
      method: 'GET',
      url: '/api/app/arrival-estimates',
      params: { logisticsDetailId, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: UpdateArrivalEstimateDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ArrivalEstimateDto>({
      method: 'PUT',
      url: `/api/app/arrival-estimates/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}