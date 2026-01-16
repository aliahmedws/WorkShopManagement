import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { GuidLookupDto, LookupService } from 'src/app/proxy/lookups';
import { CarBayDto, CarBayService, Priority } from 'src/app/proxy/car-bays';
import { GetCarListInput } from 'src/app/proxy/cars';
import { ProductionDetailsModal } from './production-details-modal/production-details-modal';
import { ConfirmationHelperService } from 'src/app/shared/services/confirmation-helper.service';
import { StageBayDto, StageDto, StageService } from 'src/app/proxy/stages';
import { Stage } from 'src/app/proxy/cars/stages';
import { mapIssueStatusColor, mapRecallStatusColor } from 'src/app/shared/utils/stage-colors.utils';

@Component({
  selector: 'app-production',
  standalone: true,
  imports: [...SHARED_IMPORTS, ProductionDetailsModal],
  templateUrl: './production.html',
  styleUrls: ['./production.scss'],
})
export class Production implements OnInit {
  @Input() filters = {} as GetCarListInput;
  @Output() refreshRequested = new EventEmitter<void>();


  bayOptions: GuidLookupDto[] = [];
  activeCarBays: StageBayDto[] = [];
  stages: StageDto[] = [];

  carId?: string;

  Priority = Priority;

  @ViewChild('detailsModal') detailsModal?: ProductionDetailsModal;

  constructor(
    private readonly lookupService: LookupService,
    private readonly carBayService: CarBayService,
    private readonly stageService: StageService, 
  ) {}

  ngOnInit(): void {
    this.lookupService.getBays().subscribe(res => (this.bayOptions = res || []));
    this.reloadActiveBays();
    this.loadProductionStages();
  }

  loadProductionStages(): void {
  // this.stageService.getStage({
  //   stage: Stage.Production,
  //   skipCount: 0,
  //   maxResultCount: 1000,
  // } as any).subscribe(res => {
  //   this.stages = res.items || [];
  // });
}

getStageCar(carId: string) {
  return this.stages.find(x => x.carId === carId);
}


  reloadActiveBays(): void {

    this.stageService.getBays().subscribe(res => this.activeCarBays = res || []);
    // this.carBayService.getList({ isActive: true, maxResultCount: 1000 } as any)
    //   .subscribe(res => (this.activeCarBays = res?.items || []));
  }

  getAssignment(bayId: string): CarBayDto | null {
    return null;
    // return this.activeCarBays.find(x => x.bayId === bayId) ?? null;
  }

  onOpenBay(bay: StageBayDto): void {
    // const a = this.getAssignment(bay.id);
    if (!bay?.carId) return;

    // safest: detailsModal exists after view init because it's in template
    this.detailsModal?.open(bay.carId, true, false);
  }

  vinLast6(v?: string | null): string {
    if (!v) return '-';
    return v.length > 6 ? v.slice(-6) : v;
  }

  // priorityText(p?: Priority | null): string {
  //   if (p === null || p === undefined) return '-';
  //   return Priority[p];
  // }

  // trackByBayId = (_: number, bay: GuidLookupDto) => bay.id;

  onStageChanged(carId: string) {
    // this.activeCarBays = this.activeCarBays.filter(x => x.carId !== carId);
    this.reloadActiveBays();
  }

  getRecallColor(bay: StageBayDto): string {
    return mapRecallStatusColor(bay?.recallStatus);
  }

  getIssueColor(bay: StageBayDto): string {
    return mapIssueStatusColor(bay?.issueStatus);
  }

  onDetailsClosed(): void {
    this.reloadActiveBays();
    this.refreshRequested.emit();
  }
}
