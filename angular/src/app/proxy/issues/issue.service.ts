import type { GetIssueListInput, IssueDto, IssueListDto, UpsertIssueDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { ListResultDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class IssueService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getList = (input: GetIssueListInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<IssueListDto>>({
      method: 'GET',
      url: '/api/app/issues',
      params: { filter: input.filter, type: input.type, status: input.status, stage: input.stage, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getListByCar = (carId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ListResultDto<IssueDto>>({
      method: 'GET',
      url: `/api/app/issues/${carId}`,
    },
    { apiName: this.apiName,...config });
  

  upsert = (carId: string, input: UpsertIssueDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, IssueDto>({
      method: 'POST',
      url: `/api/app/issues/${carId}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}