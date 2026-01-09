import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { DamageMarkerDetails } from "./damage-marker-details/damage-marker-details";
import { IssueDto, IssueStatus } from 'src/app/proxy/issues';
import { ToasterService } from '@abp/ng.theme.shared';
import { mapIssueStatusBgColor } from '../utils/issues.utils';

@Component({
  selector: 'app-damage-marker',
  imports: [...SHARED_IMPORTS, DamageMarkerDetails],
  templateUrl: './damage-marker.html',
  styleUrl: './damage-marker.scss'
})
export class DamageMarker {
  private toaster = inject(ToasterService);

  @Input() issues: IssueDto[] = [];
  @Output() issuesChange = new EventEmitter<IssueDto[]>();

  @Output() submit = new EventEmitter<void>();

  @Input() vin: string | null;

  isModalOpen = false;

  selectedIssue = {} as IssueDto;

  get nextSrNo(): number {
    return this.issues?.length
      ? Math.max(...this.issues.map(m => m.srNo)) + 1
      : 1;
  }

  onImageClick(event: MouseEvent) {
    const imageElement = event.target as HTMLElement;
    const rect = imageElement.getBoundingClientRect();

    // Calculate click position relative to image as percentage
    const x = ((event.clientX - rect.left) / rect.width) * 100;
    const y = ((event.clientY - rect.top) / rect.height) * 100;

    this.selectedIssue = {
      srNo: this.nextSrNo,
      xPercent: x,
      yPercent: y
    } as IssueDto;

    this.isModalOpen = true;
  }

  onIssueClick(issue: IssueDto | null, event: Event) {
    event.stopPropagation(); // Prevent image click event
    if (!issue) return;
    this.selectedIssue = issue;
    this.isModalOpen = true;
  }

  onIssueSubmit(issue: IssueDto | null) {
    if (!issue) return;

    //Option 1: editing a db persisted item
    if (this.selectedIssue?.id) {
      const idx = this.issues.indexOf(this.selectedIssue);
      if (idx < 0) {
        this.toaster.error('Error occurred while updating the record. Please try again');
        return;
      }
      this.issues[idx] = issue;
      this.issuesChange.emit(this.issues);
      return;
    }

    //Option 2: creating a new item or editing a non-db persisted item
    const existing = this.issues.find(i => !i.id && i.srNo === issue.srNo);
    const idx = this.issues.indexOf(existing);
    if (idx >= 0) this.issues[idx] = issue;
    else this.issues.push(issue);
    this.issuesChange.emit(this.issues);
  }

  getMarkerBgClass(issue: IssueDto | null): string {
    return mapIssueStatusBgColor(issue?.status);
  }
}