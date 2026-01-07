import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { Shared } from '../proxy/external';
import { SHARED_IMPORTS } from '../shared/shared-imports.constants';
import { CarDto, CarService, GetCarListInput } from '../proxy/cars';
import { ConfirmationHelperService } from '../shared/services/confirmation-helper.service';
import { Incoming } from "./incoming/incoming";
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { Stage } from '../proxy/stages';
import { ExternalWarehouse } from './external-warehouse/external-warehouse';

@Component({
  selector: 'app-production-manager',
  imports: [...SHARED_IMPORTS, NzTabsModule, Incoming, ExternalWarehouse],
  templateUrl: './production-manager.html',
  styleUrl: './production-manager.scss',
  providers: [ListService],
})
export class ProductionManager implements OnInit {
  ngOnInit(): void {this.list.hookToQuery(query => ({
      ...query,
      ...this.filters
    }));
  }


  public readonly list = inject(ListService);
  filters = { stage: Stage.Incoming } as GetCarListInput;
  selectedIndex = 0;

  onTabChange(index: number): void {
    this.selectedIndex = index;
    this.list.page = 0;
    let newStage: Stage;

    switch (index) {
      case 0: newStage = Stage.Incoming; break;
      case 1: newStage = Stage.ExternalWarehouse; break;
      case 2: newStage = Stage.Production; break;
      case 3: newStage = Stage.PostProduction; break;
      case 4: newStage = Stage.AwaitingTransport; break;
      default: newStage = undefined; break;
    }
    this.filters = { ...this.filters, stage: newStage };
    this.list.get();
  }
}