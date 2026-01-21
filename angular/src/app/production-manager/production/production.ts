import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { GuidLookupDto, LookupService } from 'src/app/proxy/lookups';
import { CarBayDto, CarBayService, Priority } from 'src/app/proxy/car-bays';
import { GetCarListInput } from 'src/app/proxy/cars';
import { ProductionDetailsModal } from './production-details-modal/production-details-modal';
import { ConfirmationHelperService } from 'src/app/shared/services/confirmation-helper.service';
import { StageBayDto, StageDto, StageService } from 'src/app/proxy/stages';
import { Stage } from 'src/app/proxy/cars/stages';
import { mapIssueStatusColor, mapRecallStatusColor } from 'src/app/shared/utils/stage-colors.utils';
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';
import { Confirmation } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-production',
  standalone: true,
  imports: [...SHARED_IMPORTS, ProductionDetailsModal],
  templateUrl: './production.html',
  styleUrls: ['./production.scss'],
})
export class Production implements OnInit {
  @Input() filters = {} as GetCarListInput;
  @Output() change = new EventEmitter<void>();

  bayOptions: GuidLookupDto[] = [];
  activeCarBays: StageBayDto[] = [];
  stages: StageDto[] = [];

  carId?: string;
  // carBayId?: string;

  // bayId 

  Priority = Priority;

  isProductionDetailVisible = false;

  // @ViewChild('detailsModal') detailsModal?: ProductionDetailsModal;

  constructor(
    private readonly lookupService: LookupService,
    private readonly carBayService: CarBayService,
    private readonly stageService: StageService,
    private readonly confimration: ConfirmationHelperService,
    private readonly toaster: ToasterHelperService,
  ) {}

  ngOnInit(): void {
    this.lookupService.getBays().subscribe(res => (this.bayOptions = res || []));
    this.reloadActiveBays();
  }

  getStageCar(carId: string) {
    return this.stages.find(x => x.carId === carId);
  }

  reloadActiveBays(): void {
    this.stageService.getBays().subscribe(res => (this.activeCarBays = res || []));
  }

  getAssignment(bayId: string): CarBayDto | null {
    return null;
    // return this.activeCarBays.find(x => x.bayId === bayId) ?? null;
  }

  onOpenBay(bay: StageBayDto): void {
    // const a = this.getAssignment(bay.id);
    if (!bay?.carId) return;
    this.carId = bay.carId;
    // this.carBayId = bay.bayId;
    this.isProductionDetailVisible = true;
    // safest: detailsModal exists after view init because it's in template
    // this.detailsModal?.open(bay.carId, true, false);
    // this.
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

  onStageChanged() {
    // this.activeCarBays = this.activeCarBays.filter(x => x.carId !== carId);
    this.change.emit();
    this.reloadActiveBays();
  }

  getRecallColor(bay: StageBayDto): string {
    return mapRecallStatusColor(bay?.recallStatus);
  }

  getIssueColor(bay: StageBayDto): string {
    return mapIssueStatusColor(bay?.issueStatus);
  }

  onDetailsClosed(): void {
    this.change.emit();
    this.reloadActiveBays();
    // this.refreshRequested.emit();
  }

  onDeleteClick(bay: StageBayDto, ev: MouseEvent) {
    ev.preventDefault();
    ev.stopPropagation();

    if (!bay?.carBayId) return;

    this.confimration.confirmClearBay().subscribe(status => {
      if (status !== Confirmation.Status.confirm) return;

      this.carBayService.delete(bay.carBayId).subscribe(() => {
        this.toaster.deleted();
        this.change.emit();
        this.reloadActiveBays();
        // this.refreshRequested.emit();
      });
    });
  }

  onBayChanged() {
    this.isProductionDetailVisible = false;
    this.change.emit();
    this.reloadActiveBays();
  }
}
