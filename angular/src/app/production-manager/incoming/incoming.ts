import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
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
export class Incoming {

  // cars: PagedResultDto<CarDto> = { items: [], totalCount: 0 };

  @Input() filters: any = {};
  @Output() filtersChange = new EventEmitter<any>();
  @Input() list: ListService;

  @Input() cars: PagedResultDto<CarDto> = { items: [], totalCount: 0 };

  // isModalVisible = false;
  selectedCar = {} as CarDto;
  isRecallModalVisible = false;
  isCheckInModalVisible = false;




  // ngOnInit(): void {
  //   const carStreamCreator = (query: any) => this.carService.getList({ ...query, ...this.filters });
  //   this.list.hookToQuery(carStreamCreator).subscribe((res) => (this.cars = res));
  // }

  openRecallModal(car: CarDto): void {
    this.selectedCar = car;
    this.isRecallModalVisible = true;
  }

  openCheckInModal(car: CarDto  ): void {
    this.selectedCar = car;
    this.isCheckInModalVisible = true;
  }

  
}