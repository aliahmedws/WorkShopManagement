import type { CreateQualityGateDto, QualityGateDto, UpdateQualityGateDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class QualityGateService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateQualityGateDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, QualityGateDto>({
      method: 'POST',
      url: '/api/app/quality-gates',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/quality-gates/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, QualityGateDto>({
      method: 'GET',
      url: `/api/app/quality-gates/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, QualityGateDto[]>({
      method: 'GET',
      url: '/api/app/quality-gates',
    },
    { apiName: this.apiName,...config });
  

  update = (input: UpdateQualityGateDto, id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, QualityGateDto>({
      method: 'PUT',
      url: `/api/app/quality-gates/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}