import type { GetStageInput, StageBayDto, StageDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class StageService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getBays = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, StageBayDto[]>({
      method: 'GET',
      url: '/api/app/stages/bays',
    },
    { apiName: this.apiName,...config });
  

  getStage = (input: GetStageInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<StageDto>>({
      method: 'GET',
      url: '/api/app/stages/stage',
      params: { stage: input.stage, filter: input.filter, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
}