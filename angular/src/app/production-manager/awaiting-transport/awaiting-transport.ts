import { PagedResultDto, ListService } from '@abp/ng.core';
import { Component, EventEmitter, inject, Input, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CheckInReportModal } from 'src/app/check-in-reports/check-in-report-modal/check-in-report-modal';
import { CarBayService, CarBayDto, Priority, CreateCarBayDto } from 'src/app/proxy/car-bays';
import { CarDto } from 'src/app/proxy/cars';
import { StorageLocation } from 'src/app/proxy/cars/storage-locations';
import { LookupService, GuidLookupDto } from 'src/app/proxy/lookups';
import { Recalls } from 'src/app/recalls/recalls';
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { AvvStatusModal } from '../mini-modals/avv-status-modal/avv-status-modal';
import { EstReleaseModal } from 'src/app/cars/est-release-modal/est-release-modal';
import { ProductionActions } from '../production-actions/production-actions';
import { StageDto } from 'src/app/proxy/stages';
import {
  mapRecallStatusColor,
  mapEstReleaseStatusColor,
  mapAvvStatusColor,
  mapIssueStatusColor,
} from 'src/app/shared/utils/stage-colors.utils';
import { ProductionDetailsModal } from '../production/production-details-modal/production-details-modal';
import { QualityGatesModal } from '../production/production-details-modal/quality-gates-modal/quality-gates-modal';

@Component({
  selector: 'app-awaiting-transport',
  imports: [
    ...SHARED_IMPORTS,
    AvvStatusModal,
    EstReleaseModal,
    ProductionActions,
    Recalls,
    CheckInReportModal,
    QualityGatesModal,
  ],
  templateUrl: './awaiting-transport.html',
  styleUrl: './awaiting-transport.scss',
})
export class AwaitingTransport {
  @ViewChild('detailsModal') detailsModal!: ProductionDetailsModal; //Bay

  @ViewChild('estReleaseModal', { static: true })
  estReleaseModal!: EstReleaseModal;

  private readonly carBayService = inject(CarBayService);
  private readonly lookupService = inject(LookupService);
  private readonly fb = inject(FormBuilder);
  private readonly toaster = inject(ToasterHelperService);

  form!: FormGroup;
  StorageLocation = StorageLocation;

  @Input() stages: PagedResultDto<StageDto> = { items: [], totalCount: 0 };

  @Input() filters: any = {};
  @Output() filtersChange = new EventEmitter<any>();
  @Input() list: ListService;

  selected = {} as StageDto;
  selectedId?: string; // REMOVE THIS. Instead send the whole CarDto object

  isAssignModalVisible = false;

  bayOptions: GuidLookupDto[] = [];
  selectedCarBay = {} as CarBayDto;

  priority = Priority;

  isRecallModalVisible = false;
  isCheckInModalVisible = false;
  isAvvModalVisible = false;

  issueModalVisible: Record<string, boolean> = {};

  // Quality Gates
  isQualityGatesModalVisible = false;

  openIssueModal(row: any): void {
    if (!row?.carId) return;
    this.issueModalVisible[row.carId] = true;
  }

  loadBays() {
    if (!this.bayOptions.length) {
      this.lookupService.getBays().subscribe(res => {
        this.bayOptions = res;
      });
    }
  }

  private buildForm(): void {
    this.form = this.fb.group({
      manufactureStartDate: [
        this.selectedCarBay.manufactureStartDate || null,
        [Validators.required],
      ],
      bayId: [this.selectedCarBay.bayId || null, [Validators.required]],
      priority: [this.selectedCarBay.priority || Priority.Medium, [Validators.required]],
    });
  }

  openAssignModal(carId: string): void {
    this.selectedId = carId;
    this.loadBays();
    this.buildForm();
    this.isAssignModalVisible = true;
  }

  closeAssignModal(): void {
    this.isAssignModalVisible = false;
    // this.selectedCarBay = {};
    this.selectedId = undefined;
  }

  assignToBay(): void {
    if (!this.selectedId) return;

    this.form.markAllAsTouched();
    if (this.form.invalid) return;

    const { manufactureStartDate, bayId, priority } = this.form.value;

    const input: CreateCarBayDto = {
      carId: this.selectedId,
      bayId,
      priority,
      isActive: true,
      manufactureStartDate,
    };

    this.carBayService.create(input).subscribe(() => {
      this.toaster.assign();
      this.isAssignModalVisible = false;
      this.list.get();
    });
  }

  openRecallModal(stage: StageDto): void {
    this.selected = stage;
    this.isRecallModalVisible = true;
  }

  openCheckInModal(stage: StageDto): void {
    this.selected = stage;
    this.isCheckInModalVisible = true;
  }

  openProductionDetails(row: CarDto): void {
    if (!row.id) return;
    // this.detailsModal.open(row.id, false, true);
  }

  onStageChanged(carId: string) {
    this.list.get();
    this.toaster.success('::SuccessfullyMovedToNextStage', '::Success');
  }

  openAvvModal(stage: StageDto): void {
    this.selected = stage;
    this.isAvvModalVisible = true;
  }

  openEstReleaseModal(row: StageDto): void {
    if (!row?.carId) return;
    this.estReleaseModal.open(row.carId, row.estimatedRelease ?? null, row.vin);
  }

  getRecallColor(row: StageDto): string {
    return mapRecallStatusColor(row?.recallStatus);
  }

  getEstRelease(row: StageDto): string {
    return mapEstReleaseStatusColor(row?.estimatedRelease);
  }

  mapAvvStatus(row: StageDto): string {
    return mapAvvStatusColor(row?.avvStatus);
  }

  mapIssueStatusColor(row: StageDto): string {
    return mapIssueStatusColor(row?.issueStatus);
  }

  //QUALITY GATE START
  openQualityGates(row: StageDto): void {
    if (!row?.carId) return;
    if (!row?.carBayId) return;

    this.selected = row;
    this.isQualityGatesModalVisible = true;
  }

  //QUALITY GATE END
}
