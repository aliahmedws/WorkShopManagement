import type { CreateLogisticsDetailDto, LogisticsDetailDto, UpdateLogisticsDetailDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LogisticsDetailService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateLogisticsDetailDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LogisticsDetailDto>({
      method: 'POST',
      url: '/api/app/logistics-details',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/logistics-details/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LogisticsDetailDto>({
      method: 'GET',
      url: `/api/app/logistics-details/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getByCarId = (carId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LogisticsDetailDto>({
      method: 'GET',
      url: `/api/app/logistics-details/by-car/${carId}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, filter?: string, carId?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<LogisticsDetailDto>>({
      method: 'GET',
      url: '/api/app/logistics-details',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount, filter, carId },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: UpdateLogisticsDetailDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LogisticsDetailDto>({
      method: 'PUT',
      url: `/api/app/logistics-details/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}