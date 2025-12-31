import type { BayDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { ListResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class BayService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getList = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ListResultDto<BayDto>>({
      method: 'GET',
      url: '/api/app/bays',
    },
    { apiName: this.apiName,...config });
}