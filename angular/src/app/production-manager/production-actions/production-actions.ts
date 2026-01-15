import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { Router } from '@angular/router';
import { CarCreateEditModal } from 'src/app/cars/car-create-edit-modal/car-create-edit-modal';
import { CarImagesModal } from 'src/app/cars/car-images-modal/car-images-modal';
import { IssueModal } from 'src/app/issues/issue-modal/issue-modal';
import { CarDto } from 'src/app/proxy/cars';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';

@Component({
  selector: 'app-production-actions',
  imports: [...SHARED_IMPORTS, CarCreateEditModal, CarImagesModal, IssueModal],
  templateUrl: './production-actions.html',
  styleUrl: './production-actions.scss'
})
export class ProductionActions {
  @Input() stage: CarDto;
  @Output() change = new EventEmitter<void>();

  @Input() showEdit: boolean = true;
  @Input() showImage: boolean = true;
  @Input() showLogistics: boolean = true;
  @Input() showIssue: boolean = true;

  private router = inject(Router);

  isCarEditModalVisible: boolean = false;
  isImageModalVisible: boolean = false;
  isIssueModalVisible: boolean = false;

  editCar() {
    this.isCarEditModalVisible = true;
  }


  openImages() {
    this.isImageModalVisible = true;
  }

  manageLogistics() {
    this.router.navigate(['/logistics-details'], {
      queryParams: { carId: this.stage?.id, vin: this.stage?.vin },
    });
  }

  openIssues() {
    this.isIssueModalVisible = true;
  }

  emitChange() {
    this.change.emit();
  }
}
