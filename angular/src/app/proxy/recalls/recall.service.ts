import type { CreateRecallDto, RecallDto, UpdateRecallDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class RecallService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  addOrUpdateRecalls = (carId: string, inputs: RecallDto[], config?: Partial<Rest.Config>) =>
    this.restService.request<any, RecallDto[]>({
      method: 'POST',
      url: `/api/app/recalls/${carId}/add-or-update-recalls`,
      body: inputs,
    },
    { apiName: this.apiName,...config });
  

  create = (input: CreateRecallDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RecallDto>({
      method: 'POST',
      url: '/api/app/recalls',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/recalls/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RecallDto>({
      method: 'GET',
      url: `/api/app/recalls/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getExternalRecalls = (carId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RecallDto[]>({
      method: 'GET',
      url: `/api/app/recalls/external/${carId}`,
    },
    { apiName: this.apiName,...config });
  

  getListByCar = (carId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RecallDto[]>({
      method: 'GET',
      url: `/api/app/recalls/by-car/${carId}`,
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: UpdateRecallDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RecallDto>({
      method: 'PUT',
      url: `/api/app/recalls/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}