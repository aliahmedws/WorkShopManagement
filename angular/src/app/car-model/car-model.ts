import { PagedResultDto, ListService} from '@abp/ng.core';
import { Component, inject, OnInit } from '@angular/core';

import { CarModelDto, CarModelService, GetCarModelListDto } from '../proxy/car-models';
import { ActivatedRoute, Router } from '@angular/router';
import { SHARED_IMPORTS } from '../shared/shared-imports.constants';

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
    this.router.navigate(['/car-model']);
  }

   normilzeUrl(url?: string) {
    return (url ?? '').replace(/\\/g, '/');
  }
}
