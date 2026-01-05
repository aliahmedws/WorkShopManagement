import type { CarModelDto, GetCarModelListDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CarModelService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getList = (input: GetCarModelListDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<CarModelDto>>({
      method: 'GET',
      url: '/api/app/car-models',
      params: { filters: input.filters, modelCategoryId: input.modelCategoryId, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
}