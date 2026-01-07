
export interface TwilioSmsResponseEto {
  code?: string;
  message?: string;
  more_info?: string;
  account_sid?: string;
  api_version?: string;
  body?: string;
  date_created?: string;
  date_sent?: string;
  date_updated?: string;
  direction?: string;
  error_code?: number;
  error_message?: string;
  from?: string;
  messaging_service_sid?: string;
  num_media?: string;
  num_segments?: string;
  price?: string;
  price_unit?: string;
  sid?: string;
  status?: string;
  subresource_uris: TwilioSubresourceUrisEto;
  to?: string;
  uri?: string;
}

export interface TwilioSubresourceUrisEto {
  media?: string;
}
