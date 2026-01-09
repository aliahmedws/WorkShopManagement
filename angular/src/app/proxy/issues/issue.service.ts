import type { UpsertIssuesRequestDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class IssueService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  upsert = (carId: string, input: UpsertIssuesRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/issue/upsert/${carId}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}