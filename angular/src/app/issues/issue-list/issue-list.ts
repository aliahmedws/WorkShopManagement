import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, inject, OnInit } from '@angular/core';
import { GetIssueListInput, IssueListDto, IssueService, issueStatusOptions, issueTypeOptions } from 'src/app/proxy/issues';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { IssueStatusBadge } from "../issue-status-badge/issue-status-badge";
import { stageOptions } from 'src/app/proxy/cars/stages';

@Component({
  selector: 'app-issue-list',
  imports: [...SHARED_IMPORTS, IssueStatusBadge],
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

  ngOnInit(): void {
    const stream = (query: GetIssueListInput) => this.issueService.getList({ ...query, ...this.filters });
    this.list.hookToQuery(stream).subscribe(response => this.issues = response);
  }
}
