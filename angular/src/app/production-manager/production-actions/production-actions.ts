import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { CarCreateEditModal } from 'src/app/cars/car-create-edit-modal/car-create-edit-modal';
import { CarImagesModal } from 'src/app/cars/car-images-modal/car-images-modal';
import { IssueModal } from 'src/app/issues/issue-modal/issue-modal';
import { StageDto } from 'src/app/proxy/stages';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { AssignBay } from '../assign-bay/assign-bay';
import { ProductionDetailsModal } from '../production/production-details-modal/production-details-modal';
import { ChangeStageActions } from '../production/production-details-modal/change-stage-actions/change-stage-actions';
import { Stage } from 'src/app/proxy/cars/stages';
import { LogisticsDetailsCreateEdit } from 'src/app/logistics-details/logistics-details-create-edit/logistics-details-create-edit';

@Component({
  selector: 'app-production-actions',
  imports: [
    ...SHARED_IMPORTS,
    CarCreateEditModal,
    CarImagesModal,
    IssueModal,
    AssignBay,
    ProductionDetailsModal,
    ChangeStageActions,
    LogisticsDetailsCreateEdit
  ],
  templateUrl: './production-actions.html',
  styleUrl: './production-actions.scss',
})
export class ProductionActions {
  @Input() stage: StageDto;
  @Input() currentStage?: Stage;
  @Output() change = new EventEmitter<void>();

  @Input() showEdit: boolean = true;
  @Input() showImage: boolean = true;
  @Input() showLogistics: boolean = true;
  @Input() showIssue: boolean = true;
  @Input() showMove: boolean = true;
  @Input() showLogs: boolean = true;
  
  @Input() showAssignBay = false;
  @Input() showBayDetails = false;
  @Input() showDispatched = false;

  isCarEditModalVisible: boolean = false;
  isLogisticsModalVisible: boolean = false;
  isImageModalVisible: boolean = false;
  isAssignBayVisible: boolean = false;
  isProductionDetailVisible: boolean = false;
  isChangeStageModalVisible: boolean = false;

  @Input() isIssueModalVisible: boolean = false;
  @Output() isIssueModalVisibleChange = new EventEmitter<boolean>();

  get canAssignBay(): boolean {
    return this.showAssignBay && !this.stage.carBayId && this.stage.stage != Stage.Incoming;
  }

  editCar() {
    this.isCarEditModalVisible = true;
  }


  openImages() {
    this.isImageModalVisible = true;
  }

  manageLogistics() {
    this.isLogisticsModalVisible = true;
  }

  openIssues() {
    this.isIssueModalVisible = true;
  }

  emitChange() {
    this.change.emit();
  }

  openMoveStage() {
    this.isChangeStageModalVisible = true;
  }

  onStageMoved() {
    this.isChangeStageModalVisible = false;
    this.emitChange();
  }

  onAssignBaySubmit(): void {
    this.isAssignBayVisible = false;
    this.isProductionDetailVisible = false;
    this.emitChange();
  }
}
