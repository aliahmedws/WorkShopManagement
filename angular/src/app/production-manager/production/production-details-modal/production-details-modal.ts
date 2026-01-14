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

@Component({
  selector: 'app-production-details-modal',
  standalone: true,
  imports: [...SHARED_IMPORTS, CheckListItemsModal, CarNotesModal, IssueModal],
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

  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() stageChanged = new EventEmitter<string>();

  private readonly confirm = inject(ConfirmationHelperService);

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
