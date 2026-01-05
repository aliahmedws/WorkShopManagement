import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, inject, OnInit } from '@angular/core';
import { SHARED_IMPORTS } from '../shared/shared-imports.constants';
import { GetModelCategoryListDto, ModelCategoryDto, ModelCategoryService } from '../proxy/model-categories';
import { Router } from '@angular/router';

@Component({
  selector: 'app-model-categories',
  imports: [...SHARED_IMPORTS],
  templateUrl: './model-categories.html',
  styleUrl: './model-categories.scss',
  providers: [ListService]
})
export class ModelCategories implements OnInit {
 carModels = { items: [], totalCount: 0 } as PagedResultDto<ModelCategoryDto>;

  public readonly list = inject(ListService);
  private readonly modelCategoryService = inject(ModelCategoryService);
  private readonly router = inject(Router);

  filters = {} as GetModelCategoryListDto;

  ngOnInit(): void {
    const streamCreator = (query) => { 
      return this.modelCategoryService.getList({...query, ...this.filters});
    };

    this.list.hookToQuery(streamCreator).subscribe((response) => {
      this.carModels = response;
    });

    this.list.get();
  }

  carModel(modelCategoryId: string): void {
    this.router.navigate(['car-models'], { queryParams: { modelCategoryId }});
  }
}