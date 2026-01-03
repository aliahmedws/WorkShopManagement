import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';
import type { FileAttachmentDto } from '../entity-attachments/file-attachments/models';
import type { IFormFile } from '../microsoft/asp-net-core/http/models';

@Injectable({
  providedIn: 'root',
})
export class TempFileService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  uploadTempFiles = (files: IFormFile[], config?: Partial<Rest.Config>) =>
    this.restService.request<any, FileAttachmentDto[]>({
      method: 'POST',
      url: '/api/app/temp-file/upload-temp-files',
      body: files,
    },
    { apiName: this.apiName,...config });
}