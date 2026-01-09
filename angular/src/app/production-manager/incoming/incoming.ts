import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, inject, Input, OnInit } from '@angular/core';
import { CarService, CarDto, GetCarListInput } from 'src/app/proxy/cars';
import { Recalls } from 'src/app/recalls/recalls';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';

@Component({
  selector: 'app-incoming',
  imports: [...SHARED_IMPORTS, Recalls],
  templateUrl: './incoming.html',
  styleUrl: './incoming.scss'
})
export class Incoming implements OnInit {
  public readonly list = inject(ListService);
  private readonly carService = inject(CarService);

  cars: PagedResultDto<CarDto> = { items: [], totalCount: 0 };

  @Input() filters = {} as GetCarListInput;

  selectedId?: string;
  isRecallModalVisible = false;

  ngOnInit(): void {
    const carStreamCreator = (query: any) => this.carService.getList({ ...query, ...this.filters });
    this.list.hookToQuery(carStreamCreator).subscribe((res) => (this.cars = res));
  }

  openRecallModal(id: string): void {
    this.selectedId = id;
    this.isRecallModalVisible = true;
  }

}