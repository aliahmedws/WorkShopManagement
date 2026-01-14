import { Environment } from '@abp/ng.core';

const baseUrl = 'https://workshop-web-223915365858.australia-southeast1.run.app';

const oAuthConfig = {
  issuer: 'https://workshop-api-223915365858.australia-southeast1.run.app/',
  redirectUri: baseUrl,
  clientId: 'WorkShopManagement_App',
  responseType: 'code',
  scope: 'offline_access WorkShopManagement',
  requireHttps: true,
};

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'WorkShopManagement',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://workshop-api-223915365858.australia-southeast1.run.app',
      rootNamespace: 'WorkShopManagement',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
  remoteEnv: {
    url: '/getEnvConfig',
    mergeStrategy: 'deepmerge'
  }
} as Environment;
