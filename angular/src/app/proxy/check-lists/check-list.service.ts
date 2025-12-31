import type { CheckListDto, CreateCheckListDto, GetCheckListListDto, UpdateCheckListDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CheckListService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateCheckListDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CheckListDto>({
      method: 'POST',
      url: '/api/app/check-lists',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/check-lists/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CheckListDto>({
      method: 'GET',
      url: `/api/app/check-lists/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetCheckListListDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<CheckListDto>>({
      method: 'GET',
      url: '/api/app/check-lists',
      params: { filter: input.filter, name: input.name, position: input.position, carModelId: input.carModelId, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: UpdateCheckListDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CheckListDto>({
      method: 'PUT',
      url: `/api/app/check-lists/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}