import { Component, EventEmitter, Output, ViewChild, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { CarBayDto, CarBayService, Priority } from 'src/app/proxy/car-bays';
import { CheckListItemsModal } from '../checklist-items-modal/checklist-items-modal';
import { CarService } from 'src/app/proxy/cars';
import { Stage } from 'src/app/proxy/cars/stages';
import { ConfirmationHelperService } from 'src/app/shared/services/confirmation-helper.service';
import { Confirmation } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-production-details-modal',
  standalone: true,
  imports: [...SHARED_IMPORTS, CheckListItemsModal],
  templateUrl: './production-details-modal.html',
  styleUrls: ['./production-details-modal.scss'],
})

export class ProductionDetailsModal {
  private readonly carBayService = inject(CarBayService);
  private readonly carService = inject(CarService)
  private readonly fb = inject(FormBuilder);

  @ViewChild(CheckListItemsModal) checkListItemsModal!: CheckListItemsModal;

  visible = false;
  movingStage = false;
  allowMovetoPostProduction = true;
  allowMovetoAwaitingTransport = true;

  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() stageChanged = new EventEmitter<string>();

  private readonly confirm = inject(ConfirmationHelperService);

  carId?: string;
  details?: CarBayDto;

  Priority = Priority;

  form?: FormGroup;

  open(id: string, allowMoveToPostProduction = true, allowMovetoAwaitingTransport = true): void {
    this.carId = id;
    this.allowMovetoPostProduction = allowMoveToPostProduction;
    this.allowMovetoAwaitingTransport = allowMovetoAwaitingTransport;

    this.visible = true;
    this.visibleChange.emit(true);
    this.loadDetails();
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


  private buildForm(): void {
    // If you have quality gates fields in CarBayDto, bind them here
    // Otherwise remove form completely.
    this.form = this.fb.group({
      // example only, replace with your real fields:
      // preProduction: [this.details?.preProduction ?? null, Validators.required],
    });
  }

  vinLast6(v?: string | null): string {
    if (!v) return '-';
    return v.length > 6 ? v.slice(-6) : v;
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

}
