import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { Shared } from '../proxy/external';
import { SHARED_IMPORTS } from '../shared/shared-imports.constants';
import { CarDto, CarService, GetCarListInput } from '../proxy/cars';
import { ConfirmationHelperService } from '../shared/services/confirmation-helper.service';
import { Incoming } from "./incoming/incoming";

@Component({
  selector: 'app-production-manager',
  imports: [...SHARED_IMPORTS, Incoming],
  templateUrl: './production-manager.html',
  styleUrl: './production-manager.scss',
  providers: [ListService],
})
export class ProductionManager  {
  

  public readonly list = inject(ListService);
  filters = {} as GetCarListInput;
  // private readonly carService = inject(CarService);
  // private readonly confirmation = inject(ConfirmationHelperService);

  // 
  // @Input() list: PagedResultDto<CarDto> = { items: [], totalCount: 0 };
  // @Input() filters = {} as GetCarListInput;
  // @Output() filtersChange = new EventEmitter<GetCarListInput>();

  // // isModalVisible = false;
  // selectedId?: string;

  // ngOnInit(): void {
  //   const carStreamCreator = (query: any) => this.carService.getList({ ...query, ...this.filters });
  //   this.list.hookToQuery(carStreamCreator).subscribe((res) => (this.cars = res));
  // }

  }