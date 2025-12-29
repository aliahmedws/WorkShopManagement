import type { CarModelDto, GetCarModelListDto, UpdateCarModelDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CarModelService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/car-models/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CarModelDto>({
      method: 'GET',
      url: `/api/app/car-models/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetCarModelListDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<CarModelDto>>({
      method: 'GET',
      url: '/api/app/car-models',
      params: { filter: input.filter, name: input.name, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: UpdateCarModelDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CarModelDto>({
      method: 'PUT',
      url: `/api/app/car-models/update/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}