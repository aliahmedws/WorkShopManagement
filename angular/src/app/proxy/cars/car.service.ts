import type { CarDto, CreateCarDto, ExternalCarDetailsDto, GetCarListInput, UpdateCarDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CarService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateCarDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CarDto>({
      method: 'POST',
      url: '/api/app/cars',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/cars/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CarDto>({
      method: 'GET',
      url: `/api/app/cars/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getExternalCarDetails = (vin: string, modelYear?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ExternalCarDetailsDto>({
      method: 'POST',
      url: `/api/app/cars/external-car-details/${vin}`,
      params: { modelYear },
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetCarListInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<CarDto>>({
      method: 'GET',
      url: '/api/app/cars',
      params: { filter: input.filter, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: UpdateCarDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CarDto>({
      method: 'PUT',
      url: `/api/app/cars/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}