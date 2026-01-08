import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, inject, Input, OnInit } from '@angular/core';
import { CarService, CarDto, GetCarListInput } from 'src/app/proxy/cars';
import { StorageLocation } from 'src/app/proxy/cars/storage-locations';
import { ConfirmationHelperService } from 'src/app/shared/services/confirmation-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';

@Component({
  selector: 'app-external-warehouse',
  imports: [...SHARED_IMPORTS],
  templateUrl: './external-warehouse.html',
  styleUrl: './external-warehouse.scss'
})
export class ExternalWarehouse implements OnInit {
  public readonly list = inject(ListService);
  private readonly carService = inject(CarService);
  private readonly confirmation = inject(ConfirmationHelperService);
  StorageLocation = StorageLocation;

  cars: PagedResultDto<CarDto> = { items: [], totalCount: 0 };

  @Input() filters = {} as GetCarListInput;

  selectedId?: string;

  ngOnInit(): void {
    const carStreamCreator = (query: any) => this.carService.getList({ ...query, ...this.filters });
    this.list.hookToQuery(carStreamCreator).subscribe((res) => (this.cars = res));
  }

  }