import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { Router } from '@angular/router';
import { CarCreateEditModal } from 'src/app/cars/car-create-edit-modal/car-create-edit-modal';
import { CarImagesModal } from 'src/app/cars/car-images-modal/car-images-modal';
import { IssueModal } from 'src/app/issues/issue-modal/issue-modal';
import { CarDto } from 'src/app/proxy/cars';
import { Stage } from 'src/app/proxy/cars/stages';
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
  @Input() showManageBay = true;
  
  @Output() assignBay = new EventEmitter<string>(); // carId
  @Input() showAssignBay = true; 

  @Input() showBayDetails = true;
  @Output() bayDetails = new EventEmitter<CarDto>();

  @Input() showDispatched = false;               // turn on only on AwaitingTransport page
  @Output() dispatched = new EventEmitter<string>();

  readonly Stage = Stage;

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

  get canManageBay(): boolean {
    const s = this.stage?.stage;
    return s === Stage.ExternalWarehouse || s === Stage.ScdWarehouse;
  }

   get bayLabel(): string {
    return (this.stage as CarDto)?.bayName ?? '-';
  }

  get canAssignBay(): boolean {
    const s = this.stage?.stage;
    return s === Stage.ExternalWarehouse || s === Stage.ScdWarehouse;
  }

  requestAssignBay(): void {
    if (!this.stage?.id) return;
    this.assignBay.emit(this.stage.id);
  }

  requestDispatched(): void {
    if (!this.stage?.id) return;
    this.dispatched.emit(this.stage.id);
  }
  

  openBayDetails(): void {
    if (!this.stage) return;
    this.bayDetails.emit(this.stage);
  }
}
