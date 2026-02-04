import { PagedResultDto, ListService } from '@abp/ng.core';
import { Confirmation } from '@abp/ng.theme.shared';
import { Component, EventEmitter, inject, Input, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CheckInReport } from 'src/app/check-in-reports/check-in-report';
import { CheckInReportModal } from 'src/app/check-in-reports/check-in-report-modal/check-in-report-modal';
import {
  CarBayService,
  priorityOptions,
  CarBayDto,
  Priority,
  CreateCarBayDto,
} from 'src/app/proxy/car-bays';
import { CarService, CarDto } from 'src/app/proxy/cars';
import { Stage } from 'src/app/proxy/cars/stages';
import { StorageLocation } from 'src/app/proxy/cars/storage-locations/storage-location.enum';
import { LookupService, GuidLookupDto } from 'src/app/proxy/lookups';
import { Recalls } from 'src/app/recalls/recalls';
import { ConfirmationHelperService } from 'src/app/shared/services/confirmation-helper.service';
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { ProductionActions } from '../production-actions/production-actions';
import { EstReleaseModal } from 'src/app/cars/est-release-modal/est-release-modal';
import { CarNotesModal } from 'src/app/cars/car-notes-modal/car-notes-modal';
import { StageDto } from 'src/app/proxy/stages';
import {
  mapRecallStatusColor,
  mapNoteStatusColor,
  mapEstReleaseStatusColor,
  mapCreStatusColor,
  mapIssueStatusColor,
} from 'src/app/shared/utils/stage-colors.utils';
import { QualityGatesModal } from '../production/production-details-modal/quality-gates-modal/quality-gates-modal';
import { CreDetailModal } from '../mini-modals/cre-detail-modal/cre-detail-modal';

@Component({
  selector: 'app-scd-warehouse',
  imports: [
    ...SHARED_IMPORTS,
    Recalls,
    CheckInReportModal,
    ProductionActions,
    EstReleaseModal,
    CarNotesModal,
    QualityGatesModal,
    CreDetailModal
  ],
  templateUrl: './scd-warehouse.html',
  styleUrl: './scd-warehouse.scss',
})
export class ScdWarehouse {
  private readonly carService = inject(CarService);
  private readonly confirm = inject(ConfirmationHelperService);
  private readonly carBayService = inject(CarBayService);
  private readonly lookupService = inject(LookupService);
  private readonly fb = inject(FormBuilder);
  private readonly toaster = inject(ToasterHelperService);

  //====== ASSIGN MODAL DEFINITIONS

  selectedCarBay = {} as CarBayDto; // to review later

  // ====== ASSIGN MODAL ^

  //QUALITY GATE MODAL
  isQualityGatesModalVisible = false;

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

  issueModalVisible: Record<string, boolean> = {};
  isCreDetailModalVisible = false;

  openIssueModal(row: any): void {
    if (!row?.carId) return;
    this.issueModalVisible[row.carId] = true;
  }

  // -----^ ASSIGN BAY MODAL END ^===========

  //QUALITY GATE START
  openQualityGates(row: StageDto): void {
    if (!row?.carId) return;
    this.selected = row;
    this.isQualityGatesModalVisible = true;
  }

  //QUALITY GATE END

  openCreDetailModal(car: StageDto): void {
    this.selected = car;
    this.isCreDetailModalVisible = true;
  }
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
    this.estReleaseModal.open(row.carId, row.estimatedRelease ?? null, row.vin);
  }

  openNotesModal(row: StageDto): void {
    if (!row?.carId) return;
    this.notesModal.open(row.carId, row.notes, row.vin);
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

  mapIssueStatusColor(row: StageDto): string {
    return mapIssueStatusColor(row?.issueStatus);
  }
}
