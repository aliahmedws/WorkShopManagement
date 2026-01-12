import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { GuidLookupDto, LookupService } from 'src/app/proxy/lookups';
import { CarBayDto, CarBayService, Priority } from 'src/app/proxy/car-bays';
import { GetCarListInput } from 'src/app/proxy/cars';
import { ProductionDetailsModal } from './production-details-modal/production-details-modal';
import { ConfirmationHelperService } from 'src/app/shared/services/confirmation-helper.service';

@Component({
  selector: 'app-production',
  standalone: true,
  imports: [...SHARED_IMPORTS, ProductionDetailsModal],
  templateUrl: './production.html',
  styleUrls: ['./production.scss'],
})
export class Production implements OnInit {
  @Input() filters = {} as GetCarListInput;

  bayOptions: GuidLookupDto[] = [];
  activeCarBays: CarBayDto[] = [];

  carId?: string;

  Priority = Priority;

  @ViewChild('detailsModal') detailsModal?: ProductionDetailsModal;

  constructor(
    private readonly lookupService: LookupService,
    private readonly carBayService: CarBayService,
  ) {}

  ngOnInit(): void {
    this.lookupService.getBays().subscribe(res => (this.bayOptions = res || []));
    this.reloadActiveBays();
  }

  reloadActiveBays(): void {
    this.carBayService.getList({ isActive: true, maxResultCount: 1000 } as any)
      .subscribe(res => (this.activeCarBays = res?.items || []));
  }

  getAssignment(bayId: string): CarBayDto | null {
    return this.activeCarBays.find(x => x.bayId === bayId) ?? null;
  }

  onOpenBay(bay: GuidLookupDto): void {
    const a = this.getAssignment(bay.id);
    if (!a?.carId) return;

    // safest: detailsModal exists after view init because it's in template
    this.detailsModal?.open(a.carId, true, false);
  }

  vinLast6(v?: string | null): string {
    if (!v) return '-';
    return v.length > 6 ? v.slice(-6) : v;
  }

  priorityText(p?: Priority | null): string {
    if (p === null || p === undefined) return '-';
    return Priority[p];
  }

  trackByBayId = (_: number, bay: GuidLookupDto) => bay.id;

  onStageChanged(carId: string) {
    this.activeCarBays = this.activeCarBays.filter(x => x.carId !== carId);
    this.reloadActiveBays();
  }
}
