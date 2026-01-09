import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { DamageMarker } from "../damage-marker/damage-marker";
import { IssueDto, IssueService, IssueStatus, IssueType, UpsertIssuesRequestDto } from 'src/app/proxy/issues';
import { CarDto } from 'src/app/proxy/cars';
import { IssueStatusBadge } from "../issue-status-badge/issue-status-badge";
import { IssueFilesState } from '../utils/issue-files-state.service';
import { mapToUpsertIssueDto } from '../utils/issues.utils';
import { PermissionService } from '@abp/ng.core';

@Component({
  selector: 'app-issue-modal',
  imports: [...SHARED_IMPORTS, DamageMarker, IssueStatusBadge],
  templateUrl: './issue-modal.html',
  styleUrl: './issue-modal.scss'
})
export class IssueModal {
  private issueService = inject(IssueService);
  private filesState = inject(IssueFilesState);
  private permission = inject(PermissionService);

  @Input() visible: boolean;
  @Output() visibleChange = new EventEmitter<boolean>();

  @Input() car: CarDto | null = null;

  loading: boolean = false;

  canUpsert: boolean = false;

  modalOptions = {
    size: 'xl',
    backdrop: 'static',
    keyboard: false,
    animation: true,
  };

  issues: IssueDto[] = [];

  get missingOrBrokenItemsCount(): number {
    return this.issues.filter(i => i.status != IssueStatus.Resolved && i.type == IssueType.MissingOrBrokenPart)?.length || 0;
  };

  appear() {
    this.canUpsert = this.permission.getGrantedPolicy('WorkShopManagement.Issues.Upsert');
    this.issues = [];
    this.filesState.clearAll();

    if (!this.car?.id) {
      return;
    }

    this.loading = true;
    this.issueService
      .getListByCar(this.car.id)
      .subscribe((response) => {
        this.issues = response.items;
        this.loading = false;
      });
  }

  save() {
    const req: UpsertIssuesRequestDto = {
      items: this.issues.map(issue => {
        const tempFiles = this.filesState.get({ ...issue });
        return mapToUpsertIssueDto(issue, tempFiles)
      })
    };

    this.issueService
      .upsert(this.car.id, req)
      .subscribe(() => {
        this.close();
      })
  }

  close() {
    this.visible = false;
    this.visibleChange.emit(false);
  }
}
