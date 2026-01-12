import { Component, DestroyRef, EventEmitter, inject, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { DamageMarker } from "../damage-marker/damage-marker";
import { IssueDto, IssueService, IssueStatus, IssueType } from 'src/app/proxy/issues';
import { CarDto, CarService } from 'src/app/proxy/cars';
import { IssueStatusBadge } from "../issue-status-badge/issue-status-badge";
import { IssueStateService } from '../utils/issue-state.service';
import { forkJoin, finalize, auditTime } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-issue-modal',
  imports: [...SHARED_IMPORTS, DamageMarker, IssueStatusBadge],
  templateUrl: './issue-modal.html',
  styleUrl: './issue-modal.scss',
  providers: [IssueStateService]
})
export class IssueModal implements OnInit {
  private carService = inject(CarService);
  private issueService = inject(IssueService);
  private state = inject(IssueStateService);

  private destroyRef = inject(DestroyRef);

  @Input() visible: boolean;
  @Output() visibleChange = new EventEmitter<boolean>();

  @Input() carId: string;

  car: CarDto | null = null;

  issues: IssueDto[] = [];

  loading: boolean = false;

  modalOptions = {
    size: 'xl',
    backdrop: 'static',
    keyboard: false,
    animation: true,
  };

  get missingOrBrokenItemsCount(): number {
    return this.issues.filter(i => i.status != IssueStatus.Resolved && i.type == IssueType.MissingOrBrokenPart)?.length || 0;
  };

  ngOnInit() {
    this.state.refreshRequested$
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        auditTime(100)
      )
      .subscribe(() => {
        this.reloadIssues();
      });
  }

  close() {
    this.visible = false;
    this.visibleChange.emit(false);
  }

  appear() {
    this.loading = true;

    forkJoin({
      car: this.carService.get(this.carId),
      issues: this.issueService.getListByCar(this.carId),
    })
      .pipe(finalize(() => (this.loading = false)))
      .subscribe(({ car, issues }) => {
        this.car = car;
        this.issues = issues.items;

        this.state.setCar(car);
        this.state.setIssues(issues.items);
      });
  }

  private reloadIssues() {
    if (!this.carId) return;

    this.issueService
      .getListByCar(this.carId)
      .subscribe(res => {
        this.issues = res.items;
        this.state.setIssues(res.items);
      });
  }
}
