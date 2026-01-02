import { Component } from '@angular/core';
import { DynamicLayoutComponent } from '@abp/ng.core';
import { LoaderBarComponent } from '@abp/ng.theme.shared';
import { HttpLoaderComponent } from './shared/components/http-loader/http-loader.component';

@Component({
  selector: 'app-root',
  template: `
    <abp-loader-bar />
    <abp-dynamic-layout />
    <app-http-loader />
  `,
  imports: [LoaderBarComponent, DynamicLayoutComponent, HttpLoaderComponent],
})
export class AppComponent {}
