import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, inject, OnInit } from '@angular/core';
import { SHARED_IMPORTS } from '../shared/shared-imports.constants';
import { CarDto, CarService, GetCarListInput } from '../proxy/cars';
import { Incoming } from "./incoming/incoming";
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { Stage } from '../proxy/cars/stages';
import { ExternalWarehouse } from './external-warehouse/external-warehouse';
import { Production } from "./production/production";
import { ActivatedRoute, Router } from '@angular/router';
import { PostProduction } from './post-production/post-production';

@Component({
  selector: 'app-production-manager',
  imports: [...SHARED_IMPORTS, NzTabsModule, Incoming, ExternalWarehouse, Production, PostProduction],
  templateUrl: './production-manager.html',
  styleUrl: './production-manager.scss',
  providers: [ListService],
})
export class ProductionManager implements OnInit {
  
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  public readonly list = inject(ListService);
  private readonly carService = inject(CarService);

  cars: PagedResultDto<CarDto> = { items: [], totalCount: 0 };
  filters = { stage: Stage.Incoming } as GetCarListInput;

  selectedIndex = 0;

  ngOnInit(): void {
    const carStreamCreator = (query: any) => this.carService.getList({ ...query, ...this.filters });
    this.list.hookToQuery(carStreamCreator).subscribe((res) => (this.cars = res));

    const tabParam = this.route.snapshot.queryParamMap.get('tab');
    if (tabParam) {
      this.selectedIndex = Number(tabParam);
      this.filters.stage = this.selectedIndex + 1;
    }  else {
      this.filters.stage = Stage.Incoming;
    }
  }



  onTabChange(index: number): void {

    this.cars = { items: [], totalCount: 0 };
    this.selectedIndex = index;
    this.filters.stage = index + 1
    // this.setFiltersByIndex(index);
    // this.list.get();

     // 5. Update URL without reloading the page
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { tab: index },
      queryParamsHandling: 'merge', // Preserves other params (like sorting) if they exist
    });

    this.list.get();
  }

  // 6. Refactored logic to reuse in ngOnInit and onTabChange
  // private setFiltersByIndex(index: number): void {
  //   // this.list.page = 0;
  //   let newStage: Stage;

  //   switch (index) {
  //     case 0: newStage = Stage.Incoming; break;
  //     case 1: newStage = Stage.ExternalWarehouse; break;
  //     case 2: newStage = Stage.Production; break;
  //     case 3: newStage = Stage.PostProduction; break;
  //     case 4: newStage = Stage.AwaitingTransport; break;
  //     default: newStage = undefined; break;
  //   }
  //   this.filters = { ...this.filters, stage: newStage };
  // }
}