import type { IssueDto, UpsertIssuesRequestDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { ListResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class IssueService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getListByCar = (carId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ListResultDto<IssueDto>>({
      method: 'GET',
      url: `/api/app/issues/${carId}`,
    },
    { apiName: this.apiName,...config });
  

  upsert = (carId: string, input: UpsertIssuesRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/issues',
      params: { carId },
      body: input,
    },
    { apiName: this.apiName,...config });
}