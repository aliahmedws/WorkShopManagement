import { ListService, PagedResultDto } from '@abp/ng.core';
import { Confirmation } from '@abp/ng.theme.shared';
import { Component, EventEmitter, inject, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CheckInReportModal } from 'src/app/check-in-reports/check-in-report-modal/check-in-report-modal';
import {
  CarBayDto,
  CarBayService,
  CreateCarBayDto,
  Priority,
  priorityOptions,
} from 'src/app/proxy/car-bays';
import { CarService, CarDto } from 'src/app/proxy/cars';
import { Stage } from 'src/app/proxy/cars/stages';
// import { StorageLocation } from 'src/app/proxy/cars/storage-locations';
import { GuidLookupDto, LookupService } from 'src/app/proxy/lookups';
import { Recalls } from 'src/app/recalls/recalls';
import { ConfirmationHelperService } from 'src/app/shared/services/confirmation-helper.service';
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { ProductionActions } from '../production-actions/production-actions';
import { EstReleaseModal } from 'src/app/cars/est-release-modal/est-release-modal';
import { CarNotesModal } from 'src/app/cars/car-notes-modal/car-notes-modal';
import { StageDto } from 'src/app/proxy/stages';
import { mapRecallStatusColor, mapNoteStatusColor, mapEstReleaseStatusColor, mapCreStatusColor, mapIssueStatusColor } from 'src/app/shared/utils/stage-colors.utils';

@Component({
  selector: 'app-external-warehouse',
  imports: [...SHARED_IMPORTS, Recalls, CheckInReportModal, ProductionActions, EstReleaseModal, CarNotesModal],
  templateUrl: './external-warehouse.html',
  styleUrl: './external-warehouse.scss',
})
export class ExternalWarehouse {
  private readonly carService = inject(CarService);
  private readonly confirm = inject(ConfirmationHelperService);
  private readonly carBayService = inject(CarBayService);
  private readonly lookupService = inject(LookupService);
  private readonly fb = inject(FormBuilder);
  private readonly toaster = inject(ToasterHelperService);

  //====== ASSIGN MODAL DEFINITIONS

  form!: FormGroup;
  priorityOptions = priorityOptions;
  priority = Priority;
  bayOptions: GuidLookupDto[] = [];

  isAssignModalVisible = false;
  selectedCarBay = {} as CarBayDto;   // to review later


  // ====== ASSIGN MODAL ^

  @Input() filters: any = {};
  @Output() filtersChange = new EventEmitter<any>();
  @Input() list: ListService;
  @Input() stages: PagedResultDto<StageDto> = { items: [], totalCount: 0 };

  @ViewChild('estReleaseModal', { static: true })
  estReleaseModal!: EstReleaseModal;

  @ViewChild('notesModal', { static: true })
  notesModal!: CarNotesModal;

  selected = {} as StageDto;
  isRecallModalVisible = false;
  isCheckInModalVisible = false;
  isNotesModalVisible = false;

  isOpenIssueModal = false; // To Do Later

  // StorageLocation = StorageLocation;

  // @Input() cars: PagedResultDto<CarDto> = { items: [], totalCount: 0 };

  // @Input() filters: any = {};
  // @Output() filtersChange = new EventEmitter<any>();
  // @Input() list: ListService;

  // selectedCar = {} as CarDto;
  // selectedId?: string; // REMOVE THIS. Instead send the whole CarDto object
  // isRecallModalVisible = false;
  // isCheckInModalVisible = false;

  // @ViewChild('estReleaseModal', { static: true })
  // estReleaseModal!: EstReleaseModal;
  // // ngOnInit(): void {
  // //   const carStreamCreator = (query: any) => this.carService.getList({ ...query, ...this.filters });
  // //   this.list.hookToQuery(carStreamCreator).subscribe((res) => (this.cars = res));
  // // }

  // =============== ASSIGN BAY MODAL v

  loadBays() {
    if (!this.bayOptions.length) {
      this.lookupService.getBays().subscribe(res => {
        this.bayOptions = res;
      });
    }
  }

  private buildForm(): void {
    this.form = this.fb.group({
      bayId: [this.selectedCarBay.bayId || null, [Validators.required]],
      priority: [this.selectedCarBay.priority || Priority.Medium, [Validators.required]],
    });
  }

  openAssignModal(carId: string): void {
    this.selected.carId = carId;
    this.loadBays();
    

    this.buildForm();
    this.isAssignModalVisible = true;
  }

  closeAssignModal(): void {
    this.isAssignModalVisible = false;
    // this.selectedCarBay = {};
    this.selected.carId = undefined;
  }

  assignToBay(): void {
    if (!this.selected.carId) return;

    this.form.markAllAsTouched();
    if (this.form.invalid) return;

    this.confirm
      .confirmAction('::ConfirmAssignToBayMessage', '::ConfirmAssignToBayTitle')
      .subscribe(status => {
        if (status !== Confirmation.Status.confirm) return;

        const { bayId, priority } = this.form.value;

        const input: CreateCarBayDto = {
          carId: this.selected.carId!,
          bayId,
          priority,
          isActive: true,
        };

        this.carBayService.create(input).subscribe(() => {
          this.carService
            .changeStage(this.selected.carId!, { targetStage: Stage.Production })
            .subscribe(() => {
              this.toaster.assign();
              this.list.get();
              this.isAssignModalVisible = false;
            });
        });
      });
  }

  // -----^ ASSIGN BAY MODAL END ^===========

  // ========COMMON MODALS
  openRecallModal(car: StageDto): void {
    this.selected = car;
    this.isRecallModalVisible = true;
  }

  openCheckInModal(car: StageDto): void {
    this.selected = car;
    this.isCheckInModalVisible = true;
  }

  openEstReleaseModal(row: StageDto): void {
    if (!row?.carId) return;
    this.estReleaseModal.open(row.carId, row.estimatedRelease ?? null);
  }


  openNotesModal(row: StageDto): void {
    if (!row?.carId) return;
    this.notesModal.open(row.carId, row.notes);
  }

  openIssueModal() {
    this.isOpenIssueModal = true;
  }

  onNotesSaved(e: { carId: string; notes: string }): void {
    const row = this.stages.items?.find(x => x.carId === e.carId);
    if (row) row.notes = e.notes;
    this.list.get();
  }

  // =======COLOR MAPPING
  getRecallColor(row: StageDto): string {
      return mapRecallStatusColor(row?.recallStatus);
  }

  getNoteColor(row: StageDto): string {
      return mapNoteStatusColor(row?.notes);
  }

  getEstRelease(row: StageDto): string {
    return mapEstReleaseStatusColor(row?.estimatedRelease);
  }

  getCreStatusColor(row: StageDto): string {
    return mapCreStatusColor(row?.creStatus);
  }

  getIssueStatusColor(row: StageDto): string {
    return mapIssueStatusColor(row?.issueStatus)
  }
}
