import { authGuard, permissionGuard } from '@abp/ng.core';
import { Routes } from '@angular/router';

export const APP_ROUTES: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => import('./home/home.component').then(c => c.HomeComponent),
  },
  {
    path: 'account',
    loadChildren: () => import('@abp/ng.account').then(c => c.createRoutes()),
  },
  {
    path: 'identity',
    loadChildren: () => import('@abp/ng.identity').then(c => c.createRoutes()),
  },
  {
    path: 'tenant-management',
    loadChildren: () => import('@abp/ng.tenant-management').then(c => c.createRoutes()),
  },
  {
    path: 'setting-management',
    loadChildren: () => import('@abp/ng.setting-management').then(c => c.createRoutes()),
  },
 {
  path: 'car-model',
  loadComponent: () => import('./car-model/car-model').then(m => m.CarModel),
},
{
  path: 'cars',
  loadComponent: () => import('./cars-list/cars-list').then(m => m.CarsList),
},
{
  path: 'cars/new',
  loadComponent: () => import('./cars-list/create-car/create-car').then(m => m.CreateCar),
},
{
  path: 'cars/:id/edit',
  loadComponent: () => import('./cars-list/create-car/create-car').then(m => m.CreateCar),
},




];
