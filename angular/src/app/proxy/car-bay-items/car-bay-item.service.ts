import type { SaveCarBayItemBatchDto, SaveCarBayItemBatchResultDto } from './batch-car-bay-items/models';
import type { CarBayItemDto, CreateCarBayItemDto, GetCarBayItemListDto, UpdateCarBayItemDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CarBayItemService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateCarBayItemDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CarBayItemDto>({
      method: 'POST',
      url: '/api/app/car-bay-items',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/car-bay-items/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CarBayItemDto>({
      method: 'GET',
      url: `/api/app/car-bay-items/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetCarBayItemListDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<CarBayItemDto>>({
      method: 'GET',
      url: '/api/app/car-bay-items',
      params: { filter: input.filter, checkListItemId: input.checkListItemId, carBayId: input.carBayId, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  saveBatch = (input: SaveCarBayItemBatchDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SaveCarBayItemBatchResultDto>({
      method: 'POST',
      url: '/api/app/car-bay-items/batch',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: UpdateCarBayItemDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CarBayItemDto>({
      method: 'PUT',
      url: `/api/app/car-bay-items/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}