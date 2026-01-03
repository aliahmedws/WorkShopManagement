import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import { FileAttachmentDto } from '../proxy/entity-attachments/file-attachments';


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
        this.restService.request<any, FileAttachmentDto[]>({
            method: 'POST',
            url: `/api/app/temp-file/upload`,
            body:file
        },
    { apiName: this.apiName,...config });

    constructor(private restService: RestService) {}
}
