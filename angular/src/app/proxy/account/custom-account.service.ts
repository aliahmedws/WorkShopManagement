import type { ConfirmPhoneNumberInput, GetTwoFactorProvidersInput, SendPhoneNumberConfirmationTokenDto, SendTwoFactorCodeInput } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';
import type { RegisterDto, ResetPasswordDto, SendPasswordResetCodeDto, VerifyPasswordResetTokenInput } from '../volo/abp/account/models';
import type { IdentityUserDto } from '../volo/abp/identity/models';

@Injectable({
  providedIn: 'root',
})
export class CustomAccountService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  confirmPhoneNumber = (input: ConfirmPhoneNumberInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/custom-account/confirm-phone-number',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  getTwoFactorProviders = (input: GetTwoFactorProvidersInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, string[]>({
      method: 'GET',
      url: '/api/app/custom-account/two-factor-providers',
      params: { userId: input.userId, token: input.token },
    },
    { apiName: this.apiName,...config });
  

  register = (input: RegisterDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, IdentityUserDto>({
      method: 'POST',
      url: '/api/app/custom-account/register',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  resetPassword = (input: ResetPasswordDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/custom-account/reset-password',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  sendPasswordResetCode = (input: SendPasswordResetCodeDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/custom-account/send-password-reset-code',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  sendPhoneNumberConfirmationToken = (input: SendPhoneNumberConfirmationTokenDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/custom-account/send-phone-number-confirmation-token',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  sendTwoFactorCode = (input: SendTwoFactorCodeInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/custom-account/send-two-factor-code',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  verifyPasswordResetToken = (input: VerifyPasswordResetTokenInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, boolean>({
      method: 'POST',
      url: '/api/app/custom-account/verify-password-reset-token',
      body: input,
    },
    { apiName: this.apiName,...config });
}