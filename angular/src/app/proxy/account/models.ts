
export interface ConfirmPhoneNumberInput {
  userId: string;
  token: string;
}

export interface GetTwoFactorProvidersInput {
  userId: string;
  token: string;
}

export interface SendPhoneNumberConfirmationTokenDto {
  userId: string;
  phoneNumber?: string;
}

export interface SendTwoFactorCodeInput {
  userId: string;
  provider: string;
  token: string;
}
