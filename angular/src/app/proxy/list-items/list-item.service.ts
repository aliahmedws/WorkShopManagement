import type { CreateListItemDto, GetListItemListDto, ListItemDto, UpdateListItemDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ListItemService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateListItemDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ListItemDto>({
      method: 'POST',
      url: '/api/app/list-items',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/list-items/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ListItemDto>({
      method: 'GET',
      url: `/api/app/list-items/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetListItemListDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ListItemDto>>({
      method: 'GET',
      url: '/api/app/list-items',
      params: { checkListId: input.checkListId, filter: input.filter, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: UpdateListItemDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ListItemDto>({
      method: 'PUT',
      url: `/api/app/list-items/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}