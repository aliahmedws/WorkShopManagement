import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class SignedUrlService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getSignedReadUrl = (gcsUriOrObjectName: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, string>({
      method: 'GET',
      responseType: 'text',
      url: '/signed-url/sign',
      params: { gcsUriOrObjectName },
    },
    { apiName: this.apiName,...config });
}