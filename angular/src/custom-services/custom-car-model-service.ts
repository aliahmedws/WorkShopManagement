import { Rest, RestService } from "@abp/ng.core";
import { Injectable } from "@angular/core";
import { CarModelDto } from "src/app/proxy/car-models";

@Injectable({
    providedIn: 'root'
})
export class CustomCarModelUploadService {
    apiName = 'Default';

    constructor(private restService: RestService) {}

    uploadFormData = (formData: FormData, config?: Partial<Rest.Config>) => {
        return this.restService.request<any, CarModelDto>(
            {
                method: 'POST',
                url: '/api/app/car-models/upload',
                body: formData,
            },
            { apiName: this.apiName, ...config }
        );
    }

}