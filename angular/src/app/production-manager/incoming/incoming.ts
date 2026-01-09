import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, inject, Input, OnInit } from '@angular/core';
import { CheckInReportModal } from 'src/app/check-in-reports/check-in-report-modal/check-in-report-modal';
import { CarService, CarDto, GetCarListInput } from 'src/app/proxy/cars';
import { Recalls } from 'src/app/recalls/recalls';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';

@Component({
  selector: 'app-incoming',
  imports: [...SHARED_IMPORTS, Recalls, CheckInReportModal],
  templateUrl: './incoming.html',
  styleUrl: './incoming.scss'
})
export class Incoming implements OnInit {
  public readonly list = inject(ListService);
  private readonly carService = inject(CarService);

  cars: PagedResultDto<CarDto> = { items: [], totalCount: 0 };

  @Input() filters = {} as GetCarListInput;

  // isModalVisible = false;
  selectedCar = {} as CarDto;
  isRecallModalVisible = false;

  isCheckInModalVisible = false;

  ngOnInit(): void {
    const carStreamCreator = (query: any) => this.carService.getList({ ...query, ...this.filters });
    this.list.hookToQuery(carStreamCreator).subscribe((res) => (this.cars = res));
  }

  openRecallModal(car: CarDto): void {
    this.selectedCar = car;
    this.isRecallModalVisible = true;
  }

  openCheckInModal(car: CarDto  ): void {
    this.selectedCar = car;
    this.isCheckInModalVisible = true;
  }

}