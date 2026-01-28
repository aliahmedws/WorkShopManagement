import type { GetStageInput, StageBayDto, StageDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { ListResultDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class StageService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getAll = (filter?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ListResultDto<StageDto>>({
      method: 'GET',
      url: '/api/app/stages/all-stages',
      params: { filter },
    },
    { apiName: this.apiName,...config });
  

  getBays = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, StageBayDto[]>({
      method: 'GET',
      url: '/api/app/stages/bays',
    },
    { apiName: this.apiName,...config });
  

  getListAsExcel = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>({
      method: 'POST',
      responseType: 'blob',
      url: '/api/app/stages/excel',
    },
    { apiName: this.apiName,...config });
  

  getStage = (input: GetStageInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<StageDto>>({
      method: 'GET',
      url: '/api/app/stages/stage',
      params: { stage: input.stage, filter: input.filter, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getUseProductionClassicView = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, boolean>({
      method: 'GET',
      url: '/api/app/stages/production-classic-view',
    },
    { apiName: this.apiName,...config });
  

  setUseProductionClassicView = (useClassicView: boolean, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/stages/production-classic-view/${useClassicView}`,
    },
    { apiName: this.apiName,...config });
}