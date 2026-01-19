import { Component, EventEmitter, Input, Output, ViewChild, inject } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { CarBayDto, CarBayService, ClockInStatus, clockInStatusOptions, Priority } from 'src/app/proxy/car-bays';
import { CheckListItemsModal } from '../checklist-items-modal/checklist-items-modal';
import { CarDto, CarService } from 'src/app/proxy/cars';
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
import { CreateQualityGateDto, gateNameOptions, QualityGateDto, QualityGateService, QualityGateStatus, qualityGateStatusOptions, UpdateQualityGateDto } from 'src/app/proxy/quality-gates';
import { finalize } from 'rxjs';
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';
import { checkListProgressStatusOptions } from 'src/app/proxy/check-lists';
import { mapCheckListProgressStatusColor } from 'src/app/shared/utils/stage-colors.utils';
import { CriticalImagesModal } from './critical-images-modal/critical-images-modal';
import { Recalls } from 'src/app/recalls/recalls';


@Component({
  selector: 'app-production-details-modal',
  standalone: true,
  imports: [...SHARED_IMPORTS, CheckListItemsModal, CarNotesModal, IssueModal, CriticalImagesModal, Recalls],
  templateUrl: './production-details-modal.html',
  styleUrls: ['./production-details-modal.scss'],
})

export class ProductionDetailsModal {
  private readonly carBayService = inject(CarBayService);
  private readonly carService = inject(CarService)
  private readonly qualityGateService = inject(QualityGateService);
  private readonly toaster = inject(ToasterHelperService);
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);

  @ViewChild(CheckListItemsModal) checkListItemsModal!: CheckListItemsModal;
  @ViewChild(CarNotesModal) carNotesModal!: CarNotesModal;
  @ViewChild(IssueModal) issueModal!: IssueModal;

  @Input() visible = false;
  @Output() visibleChange = new EventEmitter<boolean>();
  clockSaving = false;
  movingStage = false;      
  isIssueModalVisible = false;
  @Input() allowMovetoPostProduction = true; // NO NEED SIMPLE GET stage of car and do validation
  @Input() allowMovetoAwaitingTransport = true;
  isRecallModalVisible = false;
  criticalImagesVisible = false;

  clockInStatusOptions = clockInStatusOptions;
  checkListProgressStatus = checkListProgressStatusOptions;
  mapCheckListProgressStatusColor = mapCheckListProgressStatusColor;

  // @Output() visibleChange = new EventEmitter<boolean>();
  @Output() stageChanged = new EventEmitter<string>();
  @Output() closed = new EventEmitter<void>();

  private readonly confirm = inject(ConfirmationHelperService);

  selectedCar?: CarDto;

  @Input() carId?: string;
  @Input() carBayId?: string;
  details?: CarBayDto;
  carNotes = '';

  Priority = Priority;
  ClockInStatus = ClockInStatus

  //GateQuality
  GateName = gateNameOptions;           // MOVE TO GATE
  QualityGateStatus = qualityGateStatusOptions; // MOVE TO GATE
  selectedStatusByGate: Record<number, number> = {}; // gateValue -> statusValue  // MOVE TO GATE
  openGateValue: number | null = null;  // MOVE TO GATE

  // persistence state
  private gateByName: Record<number, QualityGateDto | null> = {}; // gateNameValue -> dto // MOVE TO GATE
  savingGate: Record<number, boolean> = {}; // gateNameValue -> boolean // MOVE TO GATE

  form?: FormGroup;

  open(): void {
    // this.carId = id;
    // this.allowMovetoPostProduction = allowMoveToPostProduction;
    // this.allowMovetoAwaitingTransport = allowMovetoAwaitingTransport;

    // this.visible = true;
    // this.visibleChange.emit(true);
    this.loadDetails();
    this.loadCarNotes();
    this.loadSelectedCar();
  }

  loadSelectedCar() {
    if (!this.carId) return;

    this.carService.get(this.carId).subscribe(car => {
      this.selectedCar = car;
    });
  }

  openRecalls(): void {
    if (!this.carId) return;
    this.isRecallModalVisible = true;
  }


  private loadCarNotes(): void {
    if (!this.carId) return;

    this.carService.get(this.carId).subscribe((car: CarDto) => {
      this.carNotes = car.notes ?? '';
    });
  }

  close(): void {
    this.visible = false;
    this.visibleChange.emit(false);
    this.details = undefined;
    this.form = undefined;
    this.carId = undefined;

    this.allowMovetoPostProduction = true;
    this.allowMovetoAwaitingTransport = true;

    this.closed.emit();
  }

  private loadDetails(): void {
    debugger;
    if (!this.carId) return;

    this.carBayService.get(this.carId).subscribe((res: CarBayDto) => {
      this.details = res;
      this.buildForm();

      this.loadQualityGates();  // MOVE TO GATE
    });
  }

  loadQualityGates() {  // MOVE TO GATE
    const carBayId = this.details?.id;
    if (!carBayId) return;

    this.qualityGateService.getListByCarBayId(carBayId).subscribe({
      next: (list) => this.applyQualityGates(list ?? []),
      error: () => this.applyQualityGates([]),
    });
  }

  applyQualityGates(list: QualityGateDto[]) {   // MOVE TO GATE
    // Reset maps
    this.gateByName = {};
    this.selectedStatusByGate = {};

    // Store existing gates
    for (const g of list) {
      const gateNameVal = g.gateName as unknown as number;
      if (!gateNameVal) continue;

      this.gateByName[gateNameVal] = g;

      const statusVal = g.status as unknown as number;
      if (statusVal) {
        this.selectedStatusByGate[gateNameVal] = statusVal;
      }
    }

    // Optional: default any missing gates to OPEN
    // (so UI always shows a selected status)
    const openVal = QualityGateStatus.OPEN as unknown as number;

    for (const opt of this.GateName) {
      const gateNameVal = opt.value as unknown as number;
      if (!this.selectedStatusByGate[gateNameVal]) {
        this.selectedStatusByGate[gateNameVal] = openVal;
        this.gateByName[gateNameVal] = null; // not created yet
      }
    }
  }


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
    debugger;
    if (!this.carId) return;

    this.isIssueModalVisible = true; // ✅ THIS IS CORRECT
  }

  goToLogistics() {
    const carId = this.carId;
    const vin = this.details?.carVin ?? null;

    if (!this.carId) return;

    this.router.navigate(['/logistics-details'], {
      queryParams: { carId, vin }
    });
    // this.close();
  }


  private buildForm(): void {
    this.form = this.fb.group({
    });
  }


  vinLast6(v?: string | null): string {
    if (!v) return '-';
    return v.length > 6 ? v.slice(-6) : v;
  }

  private normalizeBay(bayName?: string | null): string {
    if (!bayName) return '';
    // "Bay 10" -> "10"
    return bayName.replace(/^bay\s*/i, '').trim();
  }

  private buildBayLabel(bayName?: string | null): string {
    if (!bayName) return '';

    // Accept "Bay 8" OR "8" and always output "Bay 8"
    const n = bayName.toString().replace(/^bay\s*/i, '').trim();
    return n ? `Bay ${n}` : '';
  }


  moveToPostProduction() {
    const carId = this.details?.carId;
    if (!carId || this.movingStage) return;

    this.confirm
      .confirmAction(
        '::ConfirmMoveToPostProductionMessage',
        '::ConfirmMoveToPostProductionTitle'
      )
      .subscribe((status: Confirmation.Status) => {
        if (status !== Confirmation.Status.confirm) return;

        this.movingStage = true;
        this.carService.changeStage(carId, { targetStage: Stage.PostProduction }).subscribe({
          next: () => {
            this.movingStage = false;
            this.close();
            this.stageChanged.emit(carId);
          },
          error: () => {
            this.movingStage = false;
          },
        });
      });
  }

  moveToAwaitingTransportProduction() {
    const carId = this.details?.carId;
    if (!carId || this.movingStage) return;

    this.confirm
      .confirmAction(
        '::ConfirmMoveToAwaitingTransport',
        '::ConfirmMoveToAwaitingTitle'
      )
      .subscribe((status: Confirmation.Status) => {
        if (status !== Confirmation.Status.confirm) return;

        this.movingStage = true;

        this.carService.changeStage(carId, { targetStage: Stage.AwaitingTransport }).subscribe({
          next: () => {
            this.movingStage = false;
            this.close();
            this.stageChanged.emit(carId);
          },
          error: () => {
            this.movingStage = false;
          },
        });
      });
  }

  printProductionSticker(): void {
    const fullVin = this.details?.carVin || (this.selectedCar as any)?.vin;
    if (!fullVin) return;

    const item: StickerItem = {
      stickerType: 'vehicle', // hardcoded
      vin: StickerGeneratorUtil.vinLast6(fullVin),
      bay: StickerGeneratorUtil.bayLabel(this.details?.bayName),
      type: 'ASSY',           // hardcoded
      date: StickerGeneratorUtil.formatDate(new Date()), // or manufactureStartDate if you want
      model: this.details?.modelName ?? '',
      flags: '(BNE)',         // hardcoded
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


  //GateQuality // MOVE TO GATE
  setStatus(gateValue: number, statusValue: number): void {
    this.selectedStatusByGate[gateValue] = statusValue;
    this.upsertQualityGate(gateValue, statusValue);
    this.closeGateMenu();
  }

  // MOVE TO GATE
  upsertQualityGate(gateValue: number, statusValue: number) {
    const carBayId = this.details?.id;
    if (!carBayId) return;

    if (this.savingGate[gateValue]) return;
    this.savingGate[gateValue] = true;

    const existing = this.gateByName[gateValue];

    // UPDATE
    if (existing?.id) {
      const dto: UpdateQualityGateDto = {
        gateName: gateValue,
        status: statusValue,
        carBayId: carBayId,
        concurrencyStamp: existing.concurrencyStamp, // required
      };

      this.qualityGateService
        .update(dto, existing.id)
        .pipe(finalize(() => (this.savingGate[gateValue] = false)))
        .subscribe({
          next: (updated) => {
            this.gateByName[gateValue] = updated ?? null;
            this.toaster.success('::QualityGateUpdatedSuccessfully', '::Success');
          },
          error: () => {
            // optional: revert UI by reloading
            this.loadQualityGates();
          },
        });

      return;
    }

    // CREATE
    const createDto: CreateQualityGateDto = {
      gateName: gateValue,
      status: statusValue,
      carBayId: carBayId,
    };

    this.qualityGateService
      .create(createDto)
      .pipe(finalize(() => (this.savingGate[gateValue] = false)))
      .subscribe({
        next: (created) => {
          this.gateByName[gateValue] = created ?? null;
          this.toaster.success('::QualityGateCreatedSuccessfully', '::Success');
        },
        error: () => {
          // optional: revert UI by reloading
          this.loadQualityGates();
        },
      });
  }

// MOVE TO GATE
  isSelected(gateValue: number, statusValue: number): boolean {
    return this.selectedStatusByGate[gateValue] === statusValue;
  }

  // MOVE TO GATE
  dotClass(statusValue: number): string {
    switch (statusValue) {
      case 1: return 'dot-passed';
      case 2: return 'dot-major';
      case 3: return 'dot-minor';
      case 4: return 'dot-open';
      case 5: return 'dot-reset';
      default: return 'dot-reset';
    }
  }

  // MOVE TO GATE
  toggleGate(gateValue: number): void {
    this.openGateValue = this.openGateValue === gateValue ? null : gateValue;
  }

  // MOVE TO GATE
  closeGateMenu(): void {
    this.openGateValue = null;
  }

  // MOVE TO GATE
  gateBtnClass(gateValue: number): string {
    const statusValue = this.selectedStatusByGate[gateValue];

    switch (statusValue) {
      case QualityGateStatus.PASSED:
        return 'gate-passed';
      case QualityGateStatus.CONDITIONALPASSEDMAJOR:
        return 'gate-major';
      case QualityGateStatus.CONDITIONALPASSEDMINOR:
        return 'gate-minor';
      case QualityGateStatus.OPEN:
        return 'gate-open';
      case QualityGateStatus.RESET:
        return 'gate-reset';
      default:
        return 'gate-reset';
    }
  }

  get clockStatus(): ClockInStatus {
    return ((this.details)?.clockInStatus ?? ClockInStatus.NotClockedIn) as ClockInStatus;
  }

  get clockButtonKey(): string {
    return this.clockStatus === ClockInStatus.ClockedIn ? '::ClockOut' : '::ClockIn';
  }

  clockToggle(): void {
    const carBayId = this.details?.id;
    if (!carBayId || this.clockSaving) return;

    const wasClockedIn =
      Number(this.details?.clockInStatus) === ClockInStatus.ClockedIn;

    const nowIso = new Date().toISOString();

    this.clockSaving = true;

    this.carBayService.toggleClock(carBayId, nowIso).subscribe({
      next: (updated) => {
        if (!this.details) return;

        this.details = {
          ...this.details,
          clockInStatus: updated?.clockInStatus ?? this.details.clockInStatus,
          clockInTime: updated?.clockInTime ?? this.details.clockInTime,
          clockOutTime: updated?.clockOutTime ?? this.details.clockOutTime,
        } as CarBayDto;

        this.toaster.success(
          wasClockedIn ? '::ClockedOutSuccessfully' : '::ClockedInSuccessfully',
          '::Success'
        );
      },
      error: () => { },
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



}