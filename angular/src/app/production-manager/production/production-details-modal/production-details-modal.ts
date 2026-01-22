import { Component, EventEmitter, Input, Output, ViewChild, inject } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import {
  CarBayDto,
  CarBayService,
  ClockInStatus,
  clockInStatusOptions,
  Priority,
} from 'src/app/proxy/car-bays';
import { CheckListItemsModal } from '../checklist-items-modal/checklist-items-modal';
import { CarDto, CarService, Port } from 'src/app/proxy/cars';
import { Stage } from 'src/app/proxy/cars/stages';
import { ConfirmationHelperService } from 'src/app/shared/services/confirmation-helper.service';
import { Confirmation } from '@abp/ng.theme.shared';
import { CarNotesModal } from 'src/app/cars/car-notes-modal/car-notes-modal';
import { IssueModal } from 'src/app/issues/issue-modal/issue-modal';
import { Router } from '@angular/router';
import { StickerItem } from 'src/app/shared/models/sticker-item';
import { StickerGeneratorUtil } from 'src/app/shared/utils/sticker-generator.util';
import { VehicleStickerV2Item } from 'src/app/shared/models/vehicle-sticker-v2';
import { VehicleStickerV2Util } from 'src/app/shared/utils/vehicle-sticker-v2.util';
import {
  gateNameOptions,
  QualityGateDto,
  QualityGateService,
  QualityGateStatus,
  qualityGateStatusOptions,
} from 'src/app/proxy/quality-gates';
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';
import { checkListProgressStatusOptions } from 'src/app/proxy/check-lists';
import { mapCheckListProgressStatusColor } from 'src/app/shared/utils/stage-colors.utils';
import { CriticalImagesModal } from './critical-images-modal/critical-images-modal';
import { Recalls } from 'src/app/recalls/recalls';
import { AssignBay } from '../../assign-bay/assign-bay';
import { StageDto } from 'src/app/proxy/stages';
import { CheckInReportModal } from 'src/app/check-in-reports/check-in-report-modal/check-in-report-modal';
import { ChangeStageActions } from './change-stage-actions/change-stage-actions';
import { QualityGates } from './quality-gates/quality-gates';

@Component({
  selector: 'app-production-details-modal',
  standalone: true,
  imports: [
    ...SHARED_IMPORTS,
    CheckListItemsModal,
    CarNotesModal,
    IssueModal,
    CriticalImagesModal,
    Recalls,
    AssignBay,
    CheckInReportModal,
    ChangeStageActions,
    QualityGates
  ],
  templateUrl: './production-details-modal.html',
  styleUrls: ['./production-details-modal.scss'],
})
export class ProductionDetailsModal {
  private readonly carBayService = inject(CarBayService);
  private readonly carService = inject(CarService);
  private readonly toaster = inject(ToasterHelperService);
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);

  @ViewChild(CheckListItemsModal) checkListItemsModal!: CheckListItemsModal;
  @ViewChild(CarNotesModal) carNotesModal!: CarNotesModal;
  @ViewChild(IssueModal) issueModal!: IssueModal;
  @ViewChild(ChangeStageActions) changeStageModal!: ChangeStageActions;

  @Input() visible = false;
  @Output() visibleChange = new EventEmitter<boolean>();
  
  clockSaving = false;
  movingStage = false;
  isIssueModalVisible = false;
  isAssignBayVisible: boolean = false;
  isCheckInModalVisible = false;
  isChangeStageModalVisible = false;

  currentStage?: Stage;

  @Input() allowMovetoPostProduction = true; // NO NEED SIMPLE GET stage of car and do validation
  @Input() allowMovetoAwaitingTransport = true;

  isRecallModalVisible = false;
  criticalImagesVisible = false;

  clockInStatusOptions = clockInStatusOptions;
  checkListProgressStatus = checkListProgressStatusOptions;
  mapCheckListProgressStatusColor = mapCheckListProgressStatusColor;

  @Output() stageChanged = new EventEmitter<string>();
  @Output() closed = new EventEmitter<void>();

  selectedCar?: CarDto;

  @Input() carId?: string;
  @Input() carBayId?: string;
  details?: CarBayDto;
  carNotes = '';

  Priority = Priority;
  ClockInStatus = ClockInStatus;

  savingGate: Record<number, boolean> = {}; // gateNameValue -> boolean // MOVE TO GATE
  manufactureDate?: string | Date | null;

  form?: FormGroup;

  open(): void {
    this.details = undefined;
    this.loadSelectedCar();
    this.loadDetails();
  }

  loadSelectedCar() {
    if (!this.carId) return;

    this.carService.get(this.carId).subscribe(car => {
      this.selectedCar = car;
      this.currentStage = car.stage;
      this.manufactureDate = car.startDate ?? null;
    });
  }

  openRecalls(): void {
    if (!this.carId) return;
    this.isRecallModalVisible = true;
  }

  close(): void {
    this.visible = false;
    this.visibleChange.emit(false);
    // this.form = undefined;
    // this.carId = undefined;

    this.allowMovetoPostProduction = true;
    this.allowMovetoAwaitingTransport = true;

    this.closed.emit();
  }

  private loadDetails(): void {
    if (!this.carId) return;

    this.carBayService.get(this.carId).subscribe((res: CarBayDto) => {
      this.details = res;
      this.buildForm();

      //  this.loadCurrentStage();
    });
  }

  //  private loadCurrentStage(): void {
  //   if (!this.carId) return;
    
  //   this.carService.get(this.carId).subscribe(car => {
  //     this.selectedCar = car;
  //     this.currentStage = car.stage; // Assuming CarDto has stage property
  //   });
  // }

  openListItem(cl: any): void {
    if (!this.carId) return;
    if (!cl?.id) return;

    this.checkListItemsModal.open(this.details?.id!, cl.id, cl.name);
  }

  openNotes(): void {
    if (!this.carId) return;
    this.carNotesModal.open(this.carId, this.carNotes);
  }

  onNotesSaved(_: string) {
    this.carNotes = this.carNotesModal.form?.value?.notes ?? '';
  }

  openIssues(): void {
    if (!this.carId) return;

    this.isIssueModalVisible = true; // ✅ THIS IS CORRECT
  }

  goToLogistics() {
    const carId = this.carId;
    const vin = this.details?.carVin ?? null;

    if (!this.carId) return;

    this.router.navigate(['/logistics-details'], {
      queryParams: { carId, vin },
    });
    // this.close();
  }

  private buildForm(): void {
    this.form = this.fb.group({});
  }

  vinLast6(v?: string | null): string {
    if (!v) return '-';
    return v.length > 6 ? v.slice(-6) : v;
  }

  printProductionSticker(): void {
    const fullVin = this.details?.carVin || (this.selectedCar as any)?.vin;
    if (!fullVin) return;

    const item: StickerItem = {
      stickerType: 'vehicle', // hardcoded
      vin: StickerGeneratorUtil.vinLast6(fullVin),
      bay: StickerGeneratorUtil.bayLabel(this.details?.bayName),
      type: 'ASSY', // hardcoded
      date: StickerGeneratorUtil.formatDate(new Date()), // or manufactureStartDate if you want
      model: this.details?.modelName ?? '',
      flags: this.details?.port ? Port[this.details.port].toUpperCase() : '', // hardcoded
      colour: this.selectedCar?.color || '',
      name: this.details?.ownerName ?? 'Dealer Stock',
    };

    StickerGeneratorUtil.openInNewTab([item]);
  }

  printReceivingSticker(): void {
    this.printProductionSticker();
  }

  printVehicleStickerV2(): void {
    const vin = this.details?.carVin || (this.selectedCar as CarDto)?.vin;
    if (!vin) return;

    // Model
    const model = (this.details as any)?.modelName || '';
    const owner = this.details?.ownerName || 'Dealer Stock';
    const color = (this.selectedCar as CarDto)?.color || '';
    const dealer = 'BNE';
    const image = (this.details as CarBayDto)?.modelImagePath;

    if (!image) {
      return;
    }

    const item: VehicleStickerV2Item = {
      vin,
      color,
      dealer,
      image,
      model,
      owner,
    };

    VehicleStickerV2Util.openInNewTab(item);
  }


  get clockStatus(): ClockInStatus {
    return (this.details?.clockInStatus ?? ClockInStatus.NotClockedIn) as ClockInStatus;
  }

  get clockButtonKey(): string {
    return this.clockStatus === ClockInStatus.ClockedIn ? '::ClockOut' : '::ClockIn';
  }

  clockToggle(): void {
    const carBayId = this.details?.id;
    if (!carBayId || this.clockSaving) return;

    const wasClockedIn = Number(this.details?.clockInStatus) === ClockInStatus.ClockedIn;

    const nowIso = new Date().toISOString();

    this.clockSaving = true;

    this.carBayService.toggleClock(carBayId, nowIso).subscribe({
      next: updated => {
        if (!this.details) return;

        this.details = {
          ...this.details,
          clockInStatus: updated?.clockInStatus ?? this.details.clockInStatus,
          clockInTime: updated?.clockInTime ?? this.details.clockInTime,
          clockOutTime: updated?.clockOutTime ?? this.details.clockOutTime,
        } as CarBayDto;

        this.loadSelectedCar();

        this.toaster.success(
          wasClockedIn ? '::ClockedOutSuccessfully' : '::ClockedInSuccessfully',
          '::Success',
        );
      },
      error: () => {},
      complete: () => (this.clockSaving = false),
    });
  }

  onChecklistSaved(): void {
    this.loadDetails();
  }

  openCriticalImages(): void {
    const cl = (this.details as CarBayDto)?.checkLists?.[0]; // replace with your actual checklist source
    if (!cl?.id) return;

    this.criticalImagesVisible = true;
  }

  openAssignBay(): void {
    if (!this.carId) return;
    this.isAssignBayVisible = true;
  }

  getSelectedStageDto(): StageDto {
    return {
      carId: this.details?.carId,
      bayId: this.details?.bayId,
      priority: this.details?.priority || Priority.Medium,
    } as StageDto;
  }

  onAssignBaySubmit(): void {
    this.loadDetails();
    this.isAssignBayVisible = false;
    this.close();
  }

  openCheckInModal(): void {
    this.isCheckInModalVisible = true;
  }

  onCheckInReportSubmit(): void {
    this.loadDetails(); // Reload the car bay details
    this.isCheckInModalVisible = false;
  }

  openMoveStageModal(): void {
    if (!this.carId) return;
    this.isChangeStageModalVisible = true;
  }

  onStageMoved(): void {
    this.loadDetails();
    this.isChangeStageModalVisible = false;
    this.close();
    this.stageChanged.emit(this.carId!);
  }
}
