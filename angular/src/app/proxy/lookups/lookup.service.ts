import type { GuidLookupDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LookupService {
  private restService = inject(RestService);
  apiName = 'Default';
  

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
}