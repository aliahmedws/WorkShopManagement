import { Component, DestroyRef, EventEmitter, inject, input, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { stageOptions } from 'src/app/proxy/cars/stages';
import { EntityAttachmentDto } from 'src/app/proxy/entity-attachments';
import { FileAttachmentDto } from 'src/app/proxy/entity-attachments/file-attachments';
import { issueDeteriorationTypeOptions, IssueDto, issueOriginStageOptions, IssueService, issueStatusOptions, issueTypeOptions, UpsertIssueDto } from 'src/app/proxy/issues';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { PermissionService } from '@abp/ng.core';
import { CarDto } from 'src/app/proxy/cars';
import { IssueStateService } from '../../utils/issue-state.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-damage-marker-details',
  imports: [...SHARED_IMPORTS],
  templateUrl: './damage-marker-details.html',
  styleUrl: './damage-marker-details.scss'
})
export class DamageMarkerDetails implements OnInit {
  private fb = inject(FormBuilder);
  private issueService = inject(IssueService);
  private permission = inject(PermissionService);
  private state = inject(IssueStateService);

  private destroyRef = inject(DestroyRef);

  @Input() visible: boolean;
  @Output() visibleChange = new EventEmitter<boolean>();

  car: CarDto | null;
  issue = {} as IssueDto;

  canUpsert: boolean = false;

  loading = false;

  issueStatusOptions = issueStatusOptions;
  issueTypeOptions = issueTypeOptions;
  issueOriginStageOptions = issueOriginStageOptions;
  issueDeteriorationTypeOptions = issueDeteriorationTypeOptions;
  stageOptions = stageOptions;

  form: FormGroup;
  tempFiles: FileAttachmentDto[] = [];
  existingFiles: EntityAttachmentDto[] = [];

  ngOnInit() {
    this.buildForm();

    this.state.car$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((car) => {
        this.car = car;
      });

    this.state.issue$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((issue) => {
        this.issue = issue;
        this.canUpsert = this.permission.getGrantedPolicy('WorkShopManagement.Issues.Upsert');
        this.buildForm();
      });
  }

  buildForm() {
    const issue = this.issue || {} as IssueDto;

    this.form = this.fb.group({
      id: [issue.id || null],
      srNo: [issue.srNo || null],
      xPercent: [issue.xPercent || null],
      yPercent: [issue.yPercent || null],
      type: [issue.type || null, Validators.required],
      status: [issue.status || null, Validators.required],
      originStage: [issue.originStage || null, Validators.required],
      deteriorationType: [issue.deteriorationType || null, Validators.required],
      description: [issue.description || null, [Validators.required, Validators.maxLength(1024)]],
      rectificationAction: [issue.rectificationAction || null, [Validators.maxLength(1024)]],
      rectificationNotes: [issue.rectificationNotes || null, [Validators.maxLength(1024)]],
      qualityControlAction: [issue.qualityControlAction || null, [Validators.maxLength(1024)]],
      qualityControlNotes: [issue.qualityControlNotes || null, [Validators.maxLength(1024)]],
      repairerAction: [issue.repairerAction || null, [Validators.maxLength(1024)]],
      repairerNotes: [issue.repairerNotes || null, [Validators.maxLength(1024)]],
    });
    this.tempFiles = [];
    this.existingFiles = issue.entityAttachments || [];

    if (!this.canUpsert) {
      this.form.disable();
    }
  }

  save() {
    if (!this.canUpsert || !this.form?.valid) {
      this.form.markAllAsTouched();
      return;
    }

    const upsert: UpsertIssueDto = {
      ...this.form.value,
      entityAttachments: this.existingFiles,
      tempFiles: this.tempFiles
    };

    this.issueService
      .upsert(this.car?.id, upsert)
      .subscribe(() => {
        this.state.requestRefresh();
        this.close();
      });
  }

  close() {
    this.visible = false;
    this.visibleChange.emit(this.visible);
  }
}
