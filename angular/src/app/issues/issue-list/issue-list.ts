import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { GetIssueListInput, IssueListDto, IssueService, issueStatusOptions, issueTypeOptions } from 'src/app/proxy/issues';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { IssueStatusBadge } from "../issue-status-badge/issue-status-badge";
import { stageOptions } from 'src/app/proxy/cars/stages';
import { IssueModal } from '../issue-modal/issue-modal';
import { ProductionDetailsModal } from 'src/app/production-manager/production/production-details-modal/production-details-modal';

@Component({
  selector: 'app-issue-list',
  imports: [...SHARED_IMPORTS, IssueStatusBadge, IssueModal, ProductionDetailsModal],
  templateUrl: './issue-list.html',
  styleUrl: './issue-list.scss',
  providers: [ListService]
})
export class IssueList implements OnInit {
  private issueService = inject(IssueService);
  public list = inject(ListService);

  filters: GetIssueListInput = { maxResultCount: 30 };

  issues: PagedResultDto<IssueListDto> = { totalCount: 0, items: [] };

  issueStatusOptions = issueStatusOptions;
  issueTypeOptions = issueTypeOptions;
  stageOptions = stageOptions;

  selected: IssueListDto | null;
  isIssueModalOpen = false;

  @ViewChild('detailsModal') detailsModal?: ProductionDetailsModal;

  ngOnInit(): void {
    const stream = (query: GetIssueListInput) => this.issueService.getList({ ...query, ...this.filters });
    this.list.hookToQuery(stream).subscribe(response => this.issues = response);
  }

  showVinDetails(issue: IssueListDto | null) {
    this.detailsModal?.open(issue?.carId, false, false);
  }

  showIssues(issue: IssueListDto | null) {
    this.selected = issue;
    this.isIssueModalOpen = true;
  }
}
