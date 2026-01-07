import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { Shared } from '../proxy/external';
import { SHARED_IMPORTS } from '../shared/shared-imports.constants';
import { CarDto, CarService, GetCarListInput } from '../proxy/cars';
import { ConfirmationHelperService } from '../shared/services/confirmation-helper.service';
import { Incoming } from "./incoming/incoming";
import { NzTabsModule } from 'ng-zorro-antd/tabs';

@Component({
  selector: 'app-production-manager',
  imports: [...SHARED_IMPORTS, NzTabsModule, Incoming],
  templateUrl: './production-manager.html',
  styleUrl: './production-manager.scss',
  providers: [ListService],
})
export class ProductionManager  {
  

  public readonly list = inject(ListService);
  filters = {} as GetCarListInput;
  selectedIndex = 0;
  }