import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, EventEmitter, inject, Input, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { CheckInReportModal } from 'src/app/check-in-reports/check-in-report-modal/check-in-report-modal';
import { CarService, CarDto } from 'src/app/proxy/cars';
import { Recalls } from 'src/app/recalls/recalls';
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { ProductionActions } from "../production-actions/production-actions";
import { NgbDateAdapter, NgbDateNativeAdapter } from '@ng-bootstrap/ng-bootstrap'; 
import { EstReleaseModal } from 'src/app/cars/est-release-modal/est-release-modal';
import { CarNotesModal } from 'src/app/cars/car-notes-modal/car-notes-modal';
import {  StageDto } from 'src/app/proxy/stages';
import { mapCreStatusColor, mapEstReleaseStatusColor, mapIssueStatusColor, mapNoteStatusColor, mapRecallStatusColor } from 'src/app/shared/utils/stage-colors.utils';

@Component({
  selector: 'app-incoming',
  imports: [...SHARED_IMPORTS, Recalls, CheckInReportModal, ProductionActions, EstReleaseModal, CarNotesModal],
  templateUrl: './incoming.html',
  styleUrl: './incoming.scss',
  providers: [{ provide: NgbDateAdapter, useClass: NgbDateNativeAdapter }],
})
export class Incoming {
  @Input() filters: any = {};
  @Output() filtersChange = new EventEmitter<any>();
  @Input() list: ListService;
  @Input() stages: PagedResultDto<StageDto> = { items: [], totalCount: 0 };

  @ViewChild('estReleaseModal', { static: true })
  estReleaseModal!: EstReleaseModal;

  @ViewChild('notesModal', { static: true })
  notesModal!: CarNotesModal;

  selected = {} as StageDto;
  isRecallModalVisible = false;
  isCheckInModalVisible = false;
  isNotesModalVisible = false;
  isIssueModalVisible = false;


  // COMMON MODALS

  openRecallModal(car: StageDto): void {
    this.selected = car;
    this.isRecallModalVisible = true;
  }

  openCheckInModal(car: StageDto): void {
    this.selected = car;
    this.isCheckInModalVisible = true;
  }

  openEstReleaseModal(row: StageDto): void {
    if (!row?.carId) return;
    this.estReleaseModal.open(row.carId, row.estimatedRelease ?? null);
  }
  openNotesModal(row: StageDto): void {
    if (!row?.carId) return;
    this.notesModal.open(row.carId, row.notes);
  }

  onNotesSaved(e: { carId: string; notes: string }): void {
    const row = this.stages.items?.find(x => x.carId === e.carId);
    if (row) row.notes = e.notes;
    this.list.get();
  }

  getRecallColor(row: StageDto): string {
      return mapRecallStatusColor(row?.recallStatus);
  }

  getNoteColor(row: StageDto): string {
      return mapNoteStatusColor(row?.notes);
  }

  getEstRelease(row: StageDto): string {
    return mapEstReleaseStatusColor(row?.estimatedRelease);
  }

  getCreStatusColor(row: StageDto): string {
    return mapCreStatusColor(row?.creStatus);
  }

  mapIssueStatusColor(row: StageDto): string {
    return mapIssueStatusColor(row?.issueStatus);
  }

  
}