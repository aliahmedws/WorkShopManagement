import { RoutesService, eLayoutType } from '@abp/ng.core';
import { inject, provideAppInitializer } from '@angular/core';

export const APP_ROUTE_PROVIDER = [
  provideAppInitializer(() => {
    configureRoutes();
  }),
];

function configureRoutes() {
  const routes = inject(RoutesService);
  routes.add([
      {
        path: '/',
        name: '::Menu:Home',
        iconClass: 'fas fa-home',
        order: 1,
        layout: eLayoutType.application,
      },
       {
      path: '/car-model',
      name: '::Menu:CarModel',
      iconClass: 'fas fa-car',
      order: 50,
      layout: eLayoutType.application,
    },
    {
      path: '/cars',
      name: '::Menu:Cars',
      iconClass: 'fas fa-car',
      order: 10,
      layout: eLayoutType.application,
    },
  ]);
}
