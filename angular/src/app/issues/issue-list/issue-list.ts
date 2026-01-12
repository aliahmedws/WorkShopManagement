import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, inject, OnInit } from '@angular/core';
import { GetIssueListInput, IssueListDto, IssueService, issueStatusOptions, issueTypeOptions } from 'src/app/proxy/issues';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { IssueStatusBadge } from "../issue-status-badge/issue-status-badge";
import { stageOptions } from 'src/app/proxy/cars/stages';
import { IssueModal } from '../issue-modal/issue-modal';

@Component({
  selector: 'app-issue-list',
  imports: [...SHARED_IMPORTS, IssueStatusBadge, IssueModal],
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

  ngOnInit(): void {
    const stream = (query: GetIssueListInput) => this.issueService.getList({ ...query, ...this.filters });
    this.list.hookToQuery(stream).subscribe(response => this.issues = response);
  }

  //TODO: Show Production Details here.s
  showVinDetails(issue: IssueListDto | null) {
    alert('TODO: Show Production Details Modal')
  }

  showIssues(issue: IssueListDto | null) {
    this.selected = issue;
    this.isIssueModalOpen = true;
  }
}
