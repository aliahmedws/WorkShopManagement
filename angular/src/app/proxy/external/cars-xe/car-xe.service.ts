import type { RecallsResponseDto, VinResponseDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CarXeService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getRecall = (vinNo: string, ct?: any, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RecallsResponseDto>({
      method: 'GET',
      url: '/api/app/car-xe/recall',
      params: { vinNo },
    },
    { apiName: this.apiName,...config });
  

  getVin = (vinNo: string, ct?: any, config?: Partial<Rest.Config>) =>
    this.restService.request<any, VinResponseDto>({
      method: 'GET',
      url: '/api/app/car-xe/vin',
      params: { vinNo },
    },
    { apiName: this.apiName,...config });
}