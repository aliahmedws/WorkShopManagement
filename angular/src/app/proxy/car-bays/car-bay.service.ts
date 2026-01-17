import type { CarBayDto, CreateCarBayDto, GetCarBayListDto, UpdateCarBayDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CarBayService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateCarBayDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CarBayDto>({
      method: 'POST',
      url: '/api/app/car-bays',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/car-bays/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CarBayDto>({
      method: 'GET',
      url: `/api/app/car-bays/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getCarBayItemImages = (carBayId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, string[]>({
      method: 'GET',
      url: `/api/app/car-bays/critical-images/${carBayId}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetCarBayListDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<CarBayDto>>({
      method: 'GET',
      url: '/api/app/car-bays',
      params: { filter: input.filter, carId: input.carId, bayId: input.bayId, isActive: input.isActive, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  toggleClock = (id: string, time?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CarBayDto>({
      method: 'POST',
      url: `/api/app/car-bays/${id}/toggle-clock`,
      params: { time },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: UpdateCarBayDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CarBayDto>({
      method: 'PUT',
      url: `/api/app/car-bays/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}