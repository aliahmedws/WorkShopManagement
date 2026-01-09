import { LocalizationPipe } from '@abp/ng.core';
import { Component, Input } from '@angular/core';
import { IssueStatus } from 'src/app/proxy/issues';
import { mapIssueStatusBgColor } from '../utils/issues.utils';

@Component({
  selector: 'app-issue-status-badge',
  imports: [LocalizationPipe],
  templateUrl: './issue-status-badge.html',
  styleUrl: './issue-status-badge.scss'
})
export class IssueStatusBadge {
  @Input({ required: true }) status: IssueStatus;

  get className(): string | null {
    return mapIssueStatusBgColor(this.status);
  }
}
