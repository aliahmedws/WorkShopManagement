import type { GuidLookupDto, IntLookupDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';
import type { SpecsResponseDto } from '../external/cars-xe/models';

@Injectable({
  providedIn: 'root',
})
export class LookupService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getBays = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, GuidLookupDto[]>({
      method: 'GET',
      url: '/api/app/lookups/bays',
    },
    { apiName: this.apiName,...config });
  

  getCarModels = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, GuidLookupDto[]>({
      method: 'GET',
      url: '/api/app/lookups/car-models',
    },
    { apiName: this.apiName,...config });
  

  getCarOwners = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, GuidLookupDto[]>({
      method: 'GET',
      url: '/api/app/lookups/car-owners',
    },
    { apiName: this.apiName,...config });
  

  getCars = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, GuidLookupDto[]>({
      method: 'GET',
      url: '/api/app/lookups/cars',
    },
    { apiName: this.apiName,...config });
  

  getExternalSpecsResponse = (vin: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SpecsResponseDto>({
      method: 'GET',
      url: `/api/app/lookups/external-specs/${vin}`,
    },
    { apiName: this.apiName,...config });
  

  getPriority = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, IntLookupDto[]>({
      method: 'GET',
      url: '/api/app/lookups/priorities',
    },
    { apiName: this.apiName,...config });
}