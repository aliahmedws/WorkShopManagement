import type { CreatePriorityDto, GetPriorityListDto, PriorityDto, UpdatePriorityDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class PriorityService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreatePriorityDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PriorityDto>({
      method: 'POST',
      url: '/api/app/priorities',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/priorities/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PriorityDto>({
      method: 'GET',
      url: `/api/app/priorities/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetPriorityListDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<PriorityDto>>({
      method: 'GET',
      url: '/api/app/priorities',
      params: { filter: input.filter, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: UpdatePriorityDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'PUT',
      url: `/api/app/priorities/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}