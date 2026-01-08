import { Component, inject, OnInit } from '@angular/core';
import { SHARED_IMPORTS } from '../shared/shared-imports.constants';
import { CheckInReportDto, CheckInReportFiltersDto, CheckInReportService } from '../proxy/check-in-reports';
import { ListService, PagedResultDto } from '@abp/ng.core';
import { ConfirmationHelperService } from '../shared/services/confirmation-helper.service';
import { CarDto, CarService } from '../proxy/cars';

@Component({
  selector: 'app-check-in-report',
  imports: [...SHARED_IMPORTS],
  templateUrl: './check-in-report.html',
  styleUrl: './check-in-report.scss',
  providers: [ListService],
})
export class CheckInReport implements OnInit {

  public readonly list = inject(ListService);
  private readonly checkInReportService = inject(CheckInReportService);
  private readonly carService = inject(CarService);
  private readonly confirmation = inject(ConfirmationHelperService);

  reports: PagedResultDto<CheckInReportDto> = { items: [], totalCount: 0};
  filters = {} as CheckInReportFiltersDto;
  cars: CarDto[] = [];
  isModalVisible: boolean = false;
  selectedId?: string;

    ngOnInit(): void {
    const carStreamCreator = (query: any) => this.checkInReportService.getList({ ...query, ...this.filters });
    this.list.hookToQuery(carStreamCreator).subscribe((res) => (this.reports = res));
  }


    create(): void {
    this.selectedId = undefined;
    this.isModalVisible = true;
  }

  edit(id: string) {
  this.selectedId = id;
  this.isModalVisible = true;
}

}
