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
      path: '/production-manager',
      name: '::Menu:ProductionManager',
      iconClass: 'fas fa-gauge',
      order: 2,
      layout: eLayoutType.application,
      requiredPolicy: 'WorkShopManagement.ProductionManager',
    },
    {
      path: '/cars',
      name: '::Menu:VehicleInStock',
      iconClass: 'fas fa-car-side',
      order: 3,
      layout: eLayoutType.application,
      requiredPolicy: 'WorkShopManagement.Cars',
    },
    {
      path: '/car-model',
      name: '::Menu:VehicleModels',
      iconClass: 'fas fa-tags',
      order: 4,
      layout: eLayoutType.application,
      requiredPolicy: 'WorkShopManagement.Vehicles',
    },
    {
      path: '/issues',
      name: '::Menu:Issues',
      iconClass: 'fas fa-cogs',
      order: 5,
      layout: eLayoutType.application,
      requiredPolicy: 'WorkShopManagement.Issues',
    },
    // {
    //   path: '/check-in-report',
    //   name: '::Menu:checkInReports',
    //   iconClass: 'fas fa-book',
    //   order: 4,
    //   layout: eLayoutType.application
    // },
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
