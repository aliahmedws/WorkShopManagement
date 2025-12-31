import { PagedResultDto, ListService, LocalizationPipe, PermissionDirective } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PageModule } from '@abp/ng.components/page';

import { CarModelDto, CarModelService, GetCarModelListDto } from '../proxy/car-models';
import { Router } from '@angular/router';

@Component({
  selector: 'app-car-model',
  imports: [CommonModule, PageModule, ThemeSharedModule, LocalizationPipe, PermissionDirective],
  templateUrl: './car-model.html',
  styleUrl: './car-model.scss',
  providers: [ListService],
})
export class CarModel implements OnInit {
  carModels = { items: [], totalCount: 0 } as PagedResultDto<CarModelDto>;

  public readonly list = inject(ListService);
  private readonly carModelService = inject(CarModelService);
  private readonly router = inject(Router);

  ngOnInit(): void {
    const streamCreator = (query) => {
      const input: GetCarModelListDto = {
        skipCount: query.skipCount,
        maxResultCount: query.maxResultCount,
        sorting: query.sorting
      };
      return this.carModelService.getList(input);
    };

    this.list.hookToQuery(streamCreator).subscribe((response) => {
      this.carModels = response;
    });
  }

  addCheckList(carModelId: string): void {
    this.router.navigate(['check-lists'], { queryParams: { carModelId }});
  }
}
