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
    path: 'cars',
    loadComponent: () => import('./cars/cars').then(m => m.Cars),
  },
  {
    path: 'new-car',
    loadComponent: () => import('./cars/car-create-edit-modal/car-create-edit-modal').then(m => m.CarCreateEditModal),
  },
  {
    path: 'vehicles',
    loadComponent: () => import('./model-categories/model-categories').then(m => m.ModelCategories),
  },
  {
    path: 'car-models',
    loadComponent: () => import('./car-model/car-model').then(m => m.CarModel),
  },
  {
    path: 'check-lists',
    loadComponent: () => import('./check-list/check-list').then(m => m.CheckList),
  },
  {
    path: 'list-items',
    loadComponent: () => import('./list-item/list-item').then(m => m.ListItem),
  },
  {
    path: 'production-manager',
    loadComponent: () => import('./production-manager/production-manager').then(m => m.ProductionManager),
  },
  {
    path: 'check-in-report',
    loadComponent: () => import('./check-in-reports/check-in-report').then(m => m.CheckInReport)
  }
];
