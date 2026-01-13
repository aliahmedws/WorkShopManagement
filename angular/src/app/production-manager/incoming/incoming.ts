import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { CheckInReportModal } from 'src/app/check-in-reports/check-in-report-modal/check-in-report-modal';
import { CarService, CarDto, GetCarListInput } from 'src/app/proxy/cars';
import { Recalls } from 'src/app/recalls/recalls';
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';

@Component({
  selector: 'app-incoming',
  imports: [...SHARED_IMPORTS, Recalls, CheckInReportModal],
  templateUrl: './incoming.html',
  styleUrl: './incoming.scss'
})
export class Incoming {
  @Input() filters: any = {};
  @Output() filtersChange = new EventEmitter<any>();
  @Input() list: ListService;
  @Input() cars: PagedResultDto<CarDto> = { items: [], totalCount: 0 };

  private readonly fb = inject(FormBuilder);
  private readonly carService = inject(CarService);
  private readonly toaster = inject(ToasterHelperService);

  notesForm!: FormGroup;

  selectedCar = {} as CarDto;
  isRecallModalVisible = false;
  isCheckInModalVisible = false;
  isNotesModalVisible = false;

  openRecallModal(car: CarDto): void {
    this.selectedCar = car;
    this.isRecallModalVisible = true;
  }

  openCheckInModal(car: CarDto  ): void {
    this.selectedCar = car;
    this.isCheckInModalVisible = true;
  }

  openNotesModal(car: CarDto): void {
  this.selectedCar = car;

  this.notesForm = this.fb.group({
    notes: [car.notes ?? ''],
  });

  this.isNotesModalVisible = true;
}

saveNotes(): void {
  if (!this.selectedCar?.id) return;

  const notes = this.notesForm.value.notes as string;

  this.carService.updateNotes(this.selectedCar.id, notes).subscribe(() => {
    this.toaster.success('::NotesUpdatedSuccessfully', '::Success');
    this.isNotesModalVisible = false;
    this.list.get();
  });
}

  
}