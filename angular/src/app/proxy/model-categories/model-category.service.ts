import type { GetModelCategoryListDto, ModelCategoryDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ModelCategoryService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getList = (input: GetModelCategoryListDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ModelCategoryDto>>({
      method: 'GET',
      url: '/api/app/model-category',
      params: { filters: input.filters, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
}