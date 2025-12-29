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
      order: 5,
      layout: eLayoutType.application,
    },
    {
      path: '/cars',
      name: '::Menu:Cars',
      iconClass: 'fas fa-car',
      order: 4,
      layout: eLayoutType.application,
    },
    {
      path: '/cars/assign-bay',
      name: '::Menu:AssignCarToBay',
      iconClass: 'fas fa-parking',
      order: 3,
      layout: eLayoutType.application,
    },
    {
      path: '/workshop',
      name: '::Menu:Workshop',
      iconClass: 'fas fa-parking',
      order: 2,
      layout: eLayoutType.application,
    }

  ]);
}
