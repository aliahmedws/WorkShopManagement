import { PagedResultDto, ListService, LocalizationPipe, PermissionDirective } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PageModule } from '@abp/ng.components/page';

import { CarModelDto, CarModelService, GetCarModelListDto } from '../proxy/car-models';
import { ActivatedRoute, Router } from '@angular/router';
import { SHARED_IMPORTS } from '../shared/shared-imports.constants';
import {
  GetModelCategoryListDto,
  ModelCategoryDto,
  ModelCategoryService,
} from '../proxy/model-categories';

@Component({
  selector: 'app-car-model',
  imports: [...SHARED_IMPORTS],
  templateUrl: './car-model.html',
  styleUrl: './car-model.scss',
  providers: [ListService],
})
export class CarModel implements OnInit {
  carModels = { items: [], totalCount: 0 } as PagedResultDto<CarModelDto>;

  public readonly list = inject(ListService);
  private readonly carModelService = inject(CarModelService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  modelCategoryId: string | null = null;

  filters = {} as GetCarModelListDto;

  ngOnInit(): void {
    this.modelCategoryId = this.route.snapshot.queryParamMap.get('modelCategoryId');

    this.filters.modelCategoryId = this.modelCategoryId ?? undefined;

    const streamCreator = (query: GetCarModelListDto) =>
      this.carModelService.getList({ ...query, ...this.filters });

    this.list.hookToQuery(streamCreator).subscribe((response: PagedResultDto<CarModelDto>) => {
      this.carModels = response;
    });

    this.list.get();
  }


  addCheckList(carModelId: string): void {
    this.router.navigate(['check-lists'], { queryParams: { carModelId, modelCategoryId: this.modelCategoryId } });
  }

   goBack(): void {
    this.router.navigate(['/vehicles', { queryParams: { modelCategoryId: this.modelCategoryId }}]);
  }
}
