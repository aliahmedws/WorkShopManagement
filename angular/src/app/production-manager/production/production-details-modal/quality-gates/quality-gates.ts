import { Component, EventEmitter, inject, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { finalize } from 'rxjs';
import { QualityGateService, gateNameOptions, qualityGateStatusOptions, QualityGateDto, QualityGateStatus, UpdateQualityGateDto, CreateQualityGateDto } from 'src/app/proxy/quality-gates';
import { ConfirmationHelperService } from 'src/app/shared/services/confirmation-helper.service';
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';

@Component({
  selector: 'app-quality-gates',
  imports: [...SHARED_IMPORTS],
  templateUrl: './quality-gates.html',
  styleUrl: './quality-gates.scss'
})
export class QualityGates implements OnChanges {
  private readonly qualityGateService = inject(QualityGateService);
  private readonly toaster = inject(ToasterHelperService);
  private readonly confirm = inject(ConfirmationHelperService);

  @Input() carBayId?: string;
  @Output() saved = new EventEmitter<void>();
  @Input() layout: 'page' | 'modal' = 'page';

  loading = false;

  GateName = gateNameOptions;
  QualityGateStatus = qualityGateStatusOptions;

  selectedStatusByGate: Record<number, number> = {};
  
  // REMOVED: openGateValue
  // REMOVED: closeGateMenu(), toggleGate() - NgbDropdown handles this now

  private gateByName: Record<number, QualityGateDto | null> = {};
  savingGate: Record<number, boolean> = {};

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['carBayId']) {
      this.resetUiState();
      if (this.carBayId) this.load();
    }
  }

  private resetUiState(): void {
    this.selectedStatusByGate = {};
    this.gateByName = {};
    this.savingGate = {};
  }

  private load(): void {
    if (!this.carBayId) return;
    this.loading = true;
    this.qualityGateService
      .getListByCarBayId(this.carBayId)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: list => this.applyQualityGates(list ?? []),
        error: () => this.applyQualityGates([]),
      });
  }

  private applyQualityGates(list: QualityGateDto[]): void {
    this.gateByName = {};
    this.selectedStatusByGate = {};

    for (const g of list) {
      const gateNameVal = g.gateName as unknown as number;
      if (!gateNameVal) continue;
      this.gateByName[gateNameVal] = g;
      const statusVal = g.status as unknown as number;
      if (statusVal) this.selectedStatusByGate[gateNameVal] = statusVal;
    }

    const openVal = QualityGateStatus.OPEN as unknown as number;

    for (const opt of this.GateName) {
      const gateNameVal = opt.value as unknown as number;
      if (!this.selectedStatusByGate[gateNameVal]) {
        this.selectedStatusByGate[gateNameVal] = openVal;
        this.gateByName[gateNameVal] = null;
      }
    }
  }

  setStatus(gateValue: number, statusValue: number): void {
    this.selectedStatusByGate[gateValue] = statusValue;
    this.upsertQualityGate(gateValue, statusValue);
    // No need to manually close; ngbDropdownItem closes it on click automatically
  }

  private upsertQualityGate(gateValue: number, statusValue: number): void {
    if (!this.carBayId) return;
    if (this.savingGate[gateValue]) return;

    this.savingGate[gateValue] = true;
    const existing = this.gateByName[gateValue];

    if (existing?.id) {
      const dto: UpdateQualityGateDto = {
        gateName: gateValue,
        status: statusValue,
        carBayId: this.carBayId,
        concurrencyStamp: existing.concurrencyStamp,
      };

      this.qualityGateService
        .update(dto, existing.id)
        .pipe(finalize(() => (this.savingGate[gateValue] = false)))
        .subscribe({
          next: updated => {
            this.load();
            this.toaster.success('::QualityGateUpdatedSuccessfully', '::Success');
            this.saved.emit();
          },
          error: () => this.load(),
        });
      return;
    }

    const createDto: CreateQualityGateDto = {
      gateName: gateValue,
      status: statusValue,
      carBayId: this.carBayId,
    };

    this.qualityGateService
      .create(createDto)
      .pipe(finalize(() => (this.savingGate[gateValue] = false)))
      .subscribe({
        next: created => {
          this.load();
          this.toaster.success('::QualityGateCreatedSuccessfully', '::Success');
          this.saved.emit();
        },
        error: () => this.load(),
      });
  }

  isSelected(gateValue: number, statusValue: number): boolean {
    return this.selectedStatusByGate[gateValue] === statusValue;
  }

  dotClass(statusValue: number): string {
    switch (statusValue) {
      case 1: return 'dot-passed';
      case 2: return 'dot-major';
      case 3: return 'dot-minor';
      case 4: return 'dot-open';
      case 5: return 'dot-reset';
      default: return 'dot-reset';
    }
  }

  gateBtnClass(gateValue: number): string {
    const statusValue = this.selectedStatusByGate[gateValue];
    switch (statusValue) {
      case QualityGateStatus.PASSED: return 'gate-passed';
      case QualityGateStatus.CONDITIONALPASSEDMAJOR: return 'gate-major';
      case QualityGateStatus.CONDITIONALPASSEDMINOR: return 'gate-minor';
      case QualityGateStatus.OPEN: return 'gate-open';
      case QualityGateStatus.RESET: return 'gate-reset';
      default: return 'gate-reset';
    }
  }

  openHelp(): void {
    this.confirm.qualityGatesHelpMessage();
  }
}