import { Confirmation, ConfirmationService, ToasterService } from '@abp/ng.theme.shared';
import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { CarBayService } from 'src/app/proxy/car-bays';
import { CarService } from 'src/app/proxy/cars';
import { Stage, stageOptions } from 'src/app/proxy/cars/stages';
import { ConfirmationHelperService } from 'src/app/shared/services/confirmation-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';

@Component({
  selector: 'app-change-stage-actions',
  standalone: true,
  imports: [...SHARED_IMPORTS],
  templateUrl: './change-stage-actions.html',
  styleUrl: './change-stage-actions.scss',
})
export class ChangeStageActions {
  private readonly carService = inject(CarService);
  private readonly carBayService = inject(CarBayService);
  private readonly toaster = inject(ToasterService);
  private readonly confirm = inject(ConfirmationHelperService);
  private readonly confirmation = inject(ConfirmationService);
  
  @Input() visible = false;
  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() stageMoved = new EventEmitter<void>();

  @Input() carId?: string;
  @Input() carBayId?: string;
  @Input() currentStage?: Stage;

  selectedStage?: Stage;
  saving = false;

  stageOptions = stageOptions;

  private readonly stagesThatClearBay: Stage[] = [
    Stage.ScdWarehouse,
  ];

  private readonly excludedStages: Stage[] = [
    Stage.Production, // Production stage is not available for moving
  ];

  get availableStages() {
     return this.stageOptions.filter(option => {
      // Exclude the current stage
      if (this.currentStage !== undefined && this.currentStage !== null) {
        if (option.value === this.currentStage) {
          return false;
        }
      }
      
      // Exclude Production stage and any other excluded stages
      if (this.excludedStages.includes(option.value)) {
        return false;
      }
      
      return true;
    });
  }

  open(): void {
    this.selectedStage = undefined;
    this.visible = true;
    this.visibleChange.emit(true);
  }

  close(): void {
    this.visible = false;
    this.visibleChange.emit(false);
    this.selectedStage = undefined;
  }

  selectStage(stage: Stage): void {
    this.selectedStage = stage;
  }

  save(): void {
    if (!this.carId || this.selectedStage === undefined || this.saving) return;

    const selectedOption = this.stageOptions.find(opt => opt.value === this.selectedStage);
    if (!selectedOption) return;

    // Get the stage name from enum (e.g., "PostProduction", "Dispatched")
    const stageName = Stage[this.selectedStage];
    const confirmMessageKey = `::ConfirmMoveTo${stageName}`;
    const confirmTitleKey = '::ConfirmStageChange';

    this.confirm
      .confirmAction(confirmMessageKey, confirmTitleKey)
      .subscribe((status: Confirmation.Status) => {
        if (status !== Confirmation.Status.confirm) return;

        this.moveToStage();
      });
  }

  private moveToStage(): void {
    if (!this.carId || this.selectedStage === undefined) return;

    this.saving = true;

    this.carService.changeStage(this.carId, { targetStage: this.selectedStage }).subscribe({
      next: () => {
        if (this.shouldClearBay(this.selectedStage!) && this.carBayId) {
          this.clearBay();
        } else {
          this.handleSuccess();
        }
      },
      error: () => {
        this.saving = false;
        this.toaster.error('::FailedToMoveStage', '::Error');
      },
    });
  }

  private shouldClearBay(stage: Stage): boolean {
    return this.stagesThatClearBay.includes(stage);
  }

  private clearBay(): void {
    if (!this.carBayId) {
      this.handleSuccess();
      return;
    }

    this.confirm.confirmClearBay().subscribe((status: Confirmation.Status) => {
      if (status !== Confirmation.Status.confirm) {
        this.confirmation.info('::StageChangedBayNotCleared', '::Info');
        this.handleSuccess();
        return;
      }

      this.carBayService.delete(this.carBayId!).subscribe({
        next: () => {
          this.toaster.success('::BayClearedSuccessfully', '::Success');
          this.handleSuccess();
        },
        error: () => {
          this.confirmation.warn('::StageChangedButBayNotCleared', '::Warning');
          this.handleSuccess();
        },
      });
    });
  }

  private handleSuccess(): void {
    this.saving = false;
    this.toaster.success('::StageChangedSuccessfully', '::Success');
    this.close();
    this.stageMoved.emit();
  }

  isStageSelected(stage: Stage): boolean {
    return this.selectedStage === stage;
  }
}