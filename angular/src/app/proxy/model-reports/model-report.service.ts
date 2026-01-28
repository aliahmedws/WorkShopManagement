import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ModelReportService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  download = (carId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>({
      method: 'GET',
      responseType: 'blob',
      url: `/api/app/model-reports/${carId}/download`,
    },
    { apiName: this.apiName,...config });
}