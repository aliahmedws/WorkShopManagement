import type { CreateRadioOptionDto, GetRadioOptionListDto, RadioOptionDto, UpdateRadioOptionDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class RadioOptionService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateRadioOptionDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RadioOptionDto>({
      method: 'POST',
      url: '/api/app/radio-options',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/radio-options/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetRadioOptionListDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RadioOptionDto[]>({
      method: 'GET',
      url: '/api/app/radio-options',
      params: { listItemId: input.listItemId },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: UpdateRadioOptionDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RadioOptionDto>({
      method: 'PUT',
      url: `/api/app/radio-options/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}