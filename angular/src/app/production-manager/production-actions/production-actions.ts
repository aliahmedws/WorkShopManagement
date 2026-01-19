import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { Router } from '@angular/router';
import { CarCreateEditModal } from 'src/app/cars/car-create-edit-modal/car-create-edit-modal';
import { CarImagesModal } from 'src/app/cars/car-images-modal/car-images-modal';
import { IssueModal } from 'src/app/issues/issue-modal/issue-modal';
import { StageDto } from 'src/app/proxy/stages';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { AssignBay } from '../assign-bay/assign-bay';
import { ProductionDetailsModal } from '../production/production-details-modal/production-details-modal';

@Component({
  selector: 'app-production-actions',
  imports: [...SHARED_IMPORTS, CarCreateEditModal, CarImagesModal, IssueModal, AssignBay, ProductionDetailsModal],
  templateUrl: './production-actions.html',
  styleUrl: './production-actions.scss',
})
export class ProductionActions {
  @Input() stage: StageDto;
  @Output() change = new EventEmitter<void>();

  @Input() showEdit: boolean = true;
  @Input() showImage: boolean = true;
  @Input() showLogistics: boolean = true;
  @Input() showIssue: boolean = true;

  @Input() showAssignBay = false;
  @Input() showBayDetails = false;
  @Input() showDispatched = false;

  private router = inject(Router);

  isCarEditModalVisible: boolean = false;
  isImageModalVisible: boolean = false;
  isAssignBayVisible: boolean = false;
  isProductionDetailVisible: boolean = false;

  @Input() isIssueModalVisible: boolean = false;
  @Output() isIssueModalVisibleChange = new EventEmitter<boolean>();
  
  editCar() {
    this.isCarEditModalVisible = true;
  }

  openImages() {
    this.isImageModalVisible = true;
  }

  manageLogistics() {
    this.router.navigate(['/logistics-details'], {
      queryParams: { carId: this.stage?.carId, vin: this.stage?.vin },
    });
  }

  openIssues() {
    this.isIssueModalVisible = true;
  }

  emitChange() {
    this.change.emit();
  }
}
