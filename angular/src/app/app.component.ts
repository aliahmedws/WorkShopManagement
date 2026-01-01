import { Component } from '@angular/core';
import { DynamicLayoutComponent } from '@abp/ng.core';
import { LoaderBarComponent } from '@abp/ng.theme.shared';
import { NgHttpLoaderComponent } from 'ng-http-loader';

@Component({
  selector: 'app-root',
  template: `
    <abp-loader-bar />
    <abp-dynamic-layout />
    <ng-http-loader spinner="sk-chasing-dots" [filteredMethods]="['get']" [backdropBackgroundColor]="'rgba(0, 0, 0, 0.1)'" [backgroundColor]="'#134d69'" [opacity]="1"></ng-http-loader>
  `,
  imports: [LoaderBarComponent, DynamicLayoutComponent, NgHttpLoaderComponent],
})
export class AppComponent {}
