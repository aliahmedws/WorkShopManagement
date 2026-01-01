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
      path: '/cars',
      name: '::Menu:Cars',
      iconClass: 'fas fa-car',
      order: 2,
      layout: eLayoutType.application,
      requiredPolicy: 'WorkShopManagement.Cars',
    },
    {
      path: '/car-models',
      name: '::Menu:CarModels',
      iconClass: 'fas fa-car',
      order: 3,
      layout: eLayoutType.application,
      requiredPolicy: 'WorkShopManagement.CarModels',
    },
    // {
    //   path: '/check-lists',
    //   name: '::Menu:CheckLists',
    //   iconClass: 'fas fa-list-check',
    //   order: 3,
    //   layout: eLayoutType.application,
    //   // requiredPolicy: 'WorkShopManagement.CheckLists',
    // },
  ]);
}
