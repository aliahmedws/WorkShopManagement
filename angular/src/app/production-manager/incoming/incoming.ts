import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, inject, Input, OnInit } from '@angular/core';
import { CarService, CarDto, GetCarListInput } from 'src/app/proxy/cars';
import { ConfirmationHelperService } from 'src/app/shared/services/confirmation-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';

@Component({
  selector: 'app-incoming',
  imports: [...SHARED_IMPORTS],
  templateUrl: './incoming.html',
  styleUrl: './incoming.scss'
})
export class Incoming implements OnInit {
  public readonly list = inject(ListService);
  private readonly carService = inject(CarService);
  private readonly confirmation = inject(ConfirmationHelperService);

  cars: PagedResultDto<CarDto> = { items: [], totalCount: 0 };

  @Input() filters = {} as GetCarListInput;

  // isModalVisible = false;
  selectedId?: string;

  ngOnInit(): void {
    const carStreamCreator = (query: any) => this.carService.getList({ ...query, ...this.filters });
    this.list.hookToQuery(carStreamCreator).subscribe((res) => (this.cars = res));
  }

  }