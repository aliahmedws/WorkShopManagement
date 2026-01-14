import { Component, EventEmitter, Output, ViewChild, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { CarBayDto, CarBayService, Priority } from 'src/app/proxy/car-bays';
import { CheckListItemsModal } from '../checklist-items-modal/checklist-items-modal';
import { CarDto, CarService } from 'src/app/proxy/cars';
import { Stage } from 'src/app/proxy/cars/stages';
import { ConfirmationHelperService } from 'src/app/shared/services/confirmation-helper.service';
import { Confirmation } from '@abp/ng.theme.shared';
import { CarNotesModal } from 'src/app/cars/car-notes-modal/car-notes-modal';
import { IssueModal } from 'src/app/issues/issue-modal/issue-modal';
import { Router } from '@angular/router';
import { Recalls } from "src/app/recalls/recalls";
import { StickerItem } from 'src/app/shared/models/sticker-item';
import { StickerGeneratorUtil } from 'src/app/shared/utils/sticker-generator.util';
import { VehicleStickerV2Item } from 'src/app/shared/models/vehicle-sticker-v2'; 
import { VehicleStickerV2Util } from 'src/app/shared/utils/vehicle-sticker-v2.util';


@Component({
  selector: 'app-production-details-modal',
  standalone: true,
  imports: [...SHARED_IMPORTS, CheckListItemsModal, CarNotesModal, IssueModal, Recalls],
  templateUrl: './production-details-modal.html',
  styleUrls: ['./production-details-modal.scss'],
})

export class ProductionDetailsModal {
  private readonly carBayService = inject(CarBayService);
  private readonly carService = inject(CarService)
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);

  @ViewChild(CheckListItemsModal) checkListItemsModal!: CheckListItemsModal;
  @ViewChild(CarNotesModal) carNotesModal!: CarNotesModal;
  @ViewChild(IssueModal) issueModal!: IssueModal;

  visible = false;
  movingStage = false;
  isIssueModalVisible = false;
  allowMovetoPostProduction = true;
  allowMovetoAwaitingTransport = true;
  isRecallModalVisible = false; 

  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() stageChanged = new EventEmitter<string>();

  private readonly confirm = inject(ConfirmationHelperService);

  selectedCar?: CarDto;

  carId?: string;
  details?: CarBayDto;
  carNotes = '';

  Priority = Priority;

  form?: FormGroup;

  open(id: string, allowMoveToPostProduction = true, allowMovetoAwaitingTransport = true): void {
    this.carId = id;
    this.allowMovetoPostProduction = allowMoveToPostProduction;
    this.allowMovetoAwaitingTransport = allowMovetoAwaitingTransport;

    this.visible = true;
    this.visibleChange.emit(true);
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
    if (!this.selectedCar?.id) return;
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
  }

  private loadDetails(): void {
    if (!this.carId) return;

    // IMPORTANT: this must exist in proxy (recommended)
    this.carBayService.get(this.carId).subscribe((res: CarBayDto) => {
      this.details = res;
      this.buildForm();
    });
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
      colour: (this.selectedCar as any)?.colour || (this.selectedCar as any)?.color || '',
      name: this.details?.ownerName ?? 'Dealer Stock',
    };

    StickerGeneratorUtil.openInNewTab([item]);
  }


  printReceivingSticker(): void {
    this.printProductionSticker();
  }

  printVehicleStickerV2(): void {
  const vin = this.details?.carVin || (this.selectedCar as any)?.vin;
  if (!vin) return;

  // Model
  const model =
    (this.details as any)?.modelName ||
    // (this.selectedCar as any)?.modelName ||
    // (this.selectedCar as any)?.model ||
    '';

  // Owner
  const owner =
    this.details?.ownerName ||
    // (this.selectedCar as any)?.ownerName ||
    // (this.selectedCar as any)?.owner ||
    'Dealer Stock';

  const color =
    (this.details as any)?.color ||
    (this.selectedCar as CarDto)?.color ||
    '';

  // Dealer (you said hardcode BNE for now)
  const dealer = 'BNE';

  const image =
    (this.details as CarBayDto)?.modelImagePath;

  if (!image) {
    // If image is mandatory for v2, stop here (otherwise it will open but show broken image)
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
}