import { ListResultDto, ListService, PagedResultDto } from '@abp/ng.core';
import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { CarNotesModal } from 'src/app/cars/car-notes-modal/car-notes-modal';
import { EstReleaseModal } from 'src/app/cars/est-release-modal/est-release-modal';
import { GetCarListInput } from 'src/app/proxy/cars';
import { Stage, stageOptions } from 'src/app/proxy/cars/stages';
import { StageService, StageDto } from 'src/app/proxy/stages';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { mapRecallStatusColor, mapNoteStatusColor, mapEstReleaseStatusColor, mapCreStatusColor, mapIssueStatusColor, mapAvvStatusColor } from 'src/app/shared/utils/stage-colors.utils';
import { ProductionActions } from '../production-actions/production-actions';
import { Recalls } from 'src/app/recalls/recalls';
import { CheckInReport } from 'src/app/check-in-reports/check-in-report';
import { CheckInReportModal } from 'src/app/check-in-reports/check-in-report-modal/check-in-report-modal';
import { Production } from '../production/production';
import { AvvStatusModal } from '../mini-modals/avv-status-modal/avv-status-modal';
import { ProductionTopbarActions } from 'src/app/production-topbar-actions/production-topbar-actions';
import { ConfirmationHelperService } from 'src/app/shared/services/confirmation-helper.service';
import { NgbAccordionModule } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-classic-view',
  imports: [...SHARED_IMPORTS, ProductionActions, Recalls, CheckInReportModal, CarNotesModal, EstReleaseModal, Production, AvvStatusModal, ProductionTopbarActions, NgbAccordionModule],
  templateUrl: './classic-view.html',
  styleUrl: './classic-view.scss',

})
export class ClassicView implements OnInit {
  private readonly confirmation = inject(ConfirmationHelperService);
  private readonly stageService = inject(StageService);
  @ViewChild('production') production: Production;

  stages: ListResultDto<StageDto> = { items: []};

  // Fetch a large batch since we are filtering client-side for the view
  filters: any = {};
  Stage = Stage;
  stageOptions = stageOptions;

  // SEARCH STATE
  searchResults: StageDto[] = [];
  isSearching = false;

  loading = false;

  // Modal State
  selected: StageDto = {};
  isRecallModalVisible = false;
  isCheckInModalVisible = false;
  isAvvModalVisible = false

  // Track issue modal visibility for specific cars (used by production-actions)
  issueModalVisible: Record<string, boolean> = {};

  @ViewChild('estReleaseModal') estReleaseModal!: EstReleaseModal;
  @ViewChild('notesModal') notesModal!: CarNotesModal;

  ngOnInit(): void {
    this.refresh(false);
  }

  refresh(reloadBay: boolean = true): void {
    this.stageService.getAll(this.filters.filter).subscribe((res) => {
      this.stages = res;
      if (reloadBay) this.production?.reloadActiveBays();
      // If a search was active (e.g. user edited a note on a search result), re-run search
      if (this.isSearching && this.filters.filter) {
        this.performSearch(this.filters.filter);
      }
    });

  }



  // --- SEARCH LOGIC ---

  // Called by app-topbar-layout (onFilter)
  onSearchTriggered(filters: any) {
    const term = filters.filter?.trim().toLowerCase();

    if (!term) {
      this.clearSearch();
      return;
    }

    this.isSearching = true;
    this.performSearch(term);
  }

  performSearch(term: string) {
    // Client-side filter on VIN, Model, or Owner
    this.searchResults = this.stages.items.filter(x =>
      (x.vin && x.vin.toLowerCase().includes(term)) ||
      (x.modelName && x.modelName.toLowerCase().includes(term)) ||
      (x.ownerName && x.ownerName.toLowerCase().includes(term))
    );
  }

  // Called by app-topbar-layout (onReset) or the Clear button
  clearSearch() {
    this.isSearching = false;
    this.searchResults = [];
    this.filters.filter = '';
  }

  getItemsForStage(stage: Stage): StageDto[] {
    return this.stages.items.filter(x => x.stage === stage);
  }

  getCountForStage(stage: Stage): number {
    return this.getItemsForStage(stage).length;
  }

  getFilteredForStage(filter: string): StageDto[] {
    return this.stages.items.filter(x => x.vin.includes(filter));
  }

  // --- Actions & Modals ---

  openCheckInModal(row: StageDto) {
    this.selected = row;
    this.isCheckInModalVisible = true;
  }

  openRecallModal(row: StageDto) {
    this.selected = row;
    this.isRecallModalVisible = true;
  }

  openNotesModal(row: StageDto) {
    if (!row?.carId) return;
    this.notesModal.open(row.carId, row.notes);
  }

  openEstReleaseModal(row: StageDto) {
    if (!row?.carId) return;
    this.estReleaseModal.open(row.carId, row.estimatedRelease ?? null);
  }

  openIssueModal(row: StageDto) {
    // This logic seems to be handled via the issueModalVisible dictionary 
    // passed to the actions component, but if clicked directly from table icon:
    if (!row?.carId) return;
    this.issueModalVisible[row.carId] = true;
  }

  onNotesSaved(e: { carId: string; notes: string }) {
    this.refresh();
  }

  openAvvModal(stage: StageDto): void {
    this.selected = stage;
    this.isAvvModalVisible = true;
  }

  // --- Color Helpers (Using your Utils) ---

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

  getIssueStatusColor(row: StageDto): string {
    return mapIssueStatusColor(row?.issueStatus);
  }

  mapAvvStatus(row: StageDto): string {
    return mapAvvStatusColor(row?.avvStatus);
  }

  helpMessage(stage: Stage) {
    this.confirmation.productionStageHelpMessage(stage);
  }
}
