import type { EntityChangeWithUsernameDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { ListResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class EntityChangeService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getChangeHistory = (entityId: string, entityTypeFullName: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ListResultDto<EntityChangeWithUsernameDto>>({
      method: 'GET',
      url: `/api/app/entity-changes/history/${entityId}/${entityTypeFullName}`,
    },
    { apiName: this.apiName,...config });
}