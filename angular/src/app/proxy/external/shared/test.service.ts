import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';
import type { NhtsaRecallByVehicleResultDto } from '../nhtsa/models';
import type { TwilioSmsResponseEto } from '../twilio/models';
import type { VpicVariableResultDto } from '../vpic/models';

@Injectable({
  providedIn: 'root',
})
export class TestService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  decodeVinExtended = (vin: string, modelYear?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, VpicVariableResultDto>({
      method: 'POST',
      url: '/api/app/test/decode-vin-extended',
      params: { vin, modelYear },
    },
    { apiName: this.apiName,...config });
  

  getRecallByVehicle = (make: string, model: string, modelYear: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, NhtsaRecallByVehicleResultDto[]>({
      method: 'GET',
      url: '/api/app/test/recall-by-vehicle',
      params: { make, model, modelYear },
    },
    { apiName: this.apiName,...config });
  

  sendSms = (to: string, body: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, TwilioSmsResponseEto>({
      method: 'POST',
      url: '/api/app/test/send-sms',
      params: { to, body },
    },
    { apiName: this.apiName,...config });
}