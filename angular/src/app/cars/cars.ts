import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, inject, OnInit } from '@angular/core';
import { CarService, CarDto, GetCarListInput } from '../proxy/cars';
import { SHARED_IMPORTS } from '../shared/shared-imports.constants';
import { CarCreateEditModal } from './car-create-edit-modal/car-create-edit-modal';
import { ConfirmationHelperService } from '../shared/services/confirmation-helper.service';
import { IssueModal } from '../issues/issue-modal/issue-modal';
import { Router } from '@angular/router';

@Component({
  selector: 'app-cars',
  imports: [...SHARED_IMPORTS, CarCreateEditModal, IssueModal],
  templateUrl: './cars.html',
  styleUrl: './cars.scss',
  providers: [ListService],
})
export class Cars implements OnInit {
  public readonly list = inject(ListService);
  private readonly carService = inject(CarService);
  private readonly confirmation = inject(ConfirmationHelperService);
  private readonly router = inject(Router); // Inject Router

  cars: PagedResultDto<CarDto> = { items: [], totalCount: 0 };

  filters = {} as GetCarListInput;

  isModalVisible = false;
  selectedId?: string;
  selectedCar: CarDto;

  isIssueModalVisible = false;
  
  ngOnInit(): void {
    const carStreamCreator = (query: any) => this.carService.getList({ ...query, ...this.filters });
    this.list.hookToQuery(carStreamCreator).subscribe((res) => (this.cars = res));
  }

  create(): void {
    this.selectedId = undefined;
    this.isModalVisible = true;
  }

  edit(id: string): void {
    this.selectedId = id;
    this.isModalVisible = true;
  }
  
  delete(id: string): void {
    this.confirmation.confirmDelete().subscribe((status) => {
      if (status !== 'confirm') return;

      this.carService.delete(id).subscribe(() => this.list.get());
    });
  }

  showIssues(car: CarDto): void {
    this.selectedCar = car;
    this.isIssueModalVisible = true;
  }

  manageLogistics(carId: string, vin: string): void {
    this.router.navigate(['/logistics-details'], { 
      queryParams: { carId: carId, vin: vin },
      // state: { vin: vin } // didnt work
    });
  }
}