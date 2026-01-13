import type { BayDto, GetBayListInput } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { ListResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class BayService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getList = (input: GetBayListInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ListResultDto<BayDto>>({
      method: 'GET',
      url: '/api/app/bays',
      params: { filter: input.filter, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  setIsActive = (id: string, isActive: boolean, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/bays/${id}/active/${isActive}`,
    },
    { apiName: this.apiName,...config });
}