import { Component, EventEmitter, inject, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { finalize } from 'rxjs';
import { QualityGateService, gateNameOptions, qualityGateStatusOptions, QualityGateDto, QualityGateStatus, UpdateQualityGateDto, CreateQualityGateDto } from 'src/app/proxy/quality-gates';
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

  @Input() carBayId?: string;
  @Output() saved = new EventEmitter<void>();

  loading = false;

  GateName = gateNameOptions;
  QualityGateStatus = qualityGateStatusOptions;

  selectedStatusByGate: Record<number, number> = {};
  openGateValue: number | null = null;

  private gateByName: Record<number, QualityGateDto | null> = {};
  savingGate: Record<number, boolean> = {};

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['carBayId']) {
      this.resetUiState();
      if (this.carBayId) this.load();
    }
  }

  private resetUiState(): void {
    this.openGateValue = null;
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

    // ensure all gates exist in UI
    const openVal = QualityGateStatus.OPEN as unknown as number;

    for (const opt of this.GateName) {
      const gateNameVal = opt.value as unknown as number;
      if (!this.selectedStatusByGate[gateNameVal]) {
        this.selectedStatusByGate[gateNameVal] = openVal;
        this.gateByName[gateNameVal] = null;
      }
    }
  }

  // UI actions
  toggleGate(gateValue: number): void {
    this.openGateValue = this.openGateValue === gateValue ? null : gateValue;
  }

  private closeGateMenu(): void {
    this.openGateValue = null;
  }

  setStatus(gateValue: number, statusValue: number): void {
    this.selectedStatusByGate[gateValue] = statusValue;
    this.upsertQualityGate(gateValue, statusValue);
    this.closeGateMenu();
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
            this.gateByName[gateValue] = updated ?? null;
            this.toaster.success('::QualityGateUpdatedSuccessfully', '::Success');
            this.saved.emit();
          },
          error: () => this.load(), // revert
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
          this.gateByName[gateValue] = created ?? null;
          this.toaster.success('::QualityGateCreatedSuccessfully', '::Success');
          this.saved.emit();
        },
        error: () => this.load(),
      });
  }

  // helpers (same logic as you had)
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
}
