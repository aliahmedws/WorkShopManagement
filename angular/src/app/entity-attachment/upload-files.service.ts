import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

export interface TempFileDto {
    name?: string;
    path?: string;
}

export interface AttachmentUiModel {
    id?: string;
    name: string;
    path: string;
    isNew: boolean;
}

@Injectable({
    providedIn: 'root',
})
export class UploadFileService {
    apiName = 'Default';

    uploadFile = (file: FormData, config?: Partial<Rest.Config>) =>
        this.restService.request<any, TempFileDto[]>({
            method: 'POST',
            url: `/api/app/temp-file/upload`,
            body:file
        },
    { apiName: this.apiName,...config });

    constructor(private restService: RestService) {}
}
