import { Component, DestroyRef, inject, OnDestroy, OnInit, Output } from '@angular/core';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { DamageMarkerDetails } from "./damage-marker-details/damage-marker-details";
import { IssueDto } from 'src/app/proxy/issues';
import { mapIssueStatusBgColor } from '../utils/issues.utils';
import { IssueStateService } from '../utils/issue-state.service';
import { CarDto } from 'src/app/proxy/cars';
import { Subject, takeUntil } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-damage-marker',
  imports: [...SHARED_IMPORTS, DamageMarkerDetails],
  templateUrl: './damage-marker.html',
  styleUrl: './damage-marker.scss'
})
export class DamageMarker implements OnInit {
  private state = inject(IssueStateService);
  private destroyRef = inject(DestroyRef);
  
  car: CarDto | null;
  issues: IssueDto[] = [];

  isModalOpen = false;

  get nextSrNo(): number {
    return this.issues?.length
      ? Math.max(...this.issues.map(m => m.srNo)) + 1
      : 1;
  }

  ngOnInit() {
    this.state.car$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((car) => {
        this.car = car;
      });

    this.state.issues$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((issues) => {
        this.issues = issues;
      });
  }

  onImageClick(event: MouseEvent) {
    const imageElement = event.target as HTMLElement;
    const rect = imageElement.getBoundingClientRect();

    // Calculate click position relative to image as percentage
    const x = ((event.clientX - rect.left) / rect.width) * 100;
    const y = ((event.clientY - rect.top) / rect.height) * 100;

    const issue = {
      srNo: this.nextSrNo,
      xPercent: x,
      yPercent: y
    } as IssueDto;

    this.state.setIssue(issue);

    this.isModalOpen = true;
  }

  onIssueClick(issue: IssueDto | null, event: Event) {
    event.stopPropagation(); // Prevent image click event
    if (!issue) return;
    this.state.setIssue(issue);
    this.isModalOpen = true;
  }

  getMarkerBgClass(issue: IssueDto | null): string {
    return mapIssueStatusBgColor(issue?.status);
  }
}