import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { stageOptions } from 'src/app/proxy/cars/stages';
import { issueDeteriorationTypeOptions, IssueDto, issueOriginStageOptions, issueStatusOptions, issueTypeOptions } from 'src/app/proxy/issues';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';

@Component({
  selector: 'app-damage-marker-details',
  imports: [...SHARED_IMPORTS],
  templateUrl: './damage-marker-details.html',
  styleUrl: './damage-marker-details.scss'
})
export class DamageMarkerDetails {
  private fb = inject(FormBuilder);

  @Input() visible: boolean;
  @Output() visibleChange = new EventEmitter<boolean>();

  @Input() vin: string | null;
  @Input() issue = {} as IssueDto;

  @Output() submit = new EventEmitter<IssueDto>();

  loading = false;

  issueStatusOptions = issueStatusOptions;
  issueTypeOptions = issueTypeOptions;
  issueOriginStageOptions = issueOriginStageOptions;
  issueDeteriorationTypeOptions = issueDeteriorationTypeOptions;
  stageOptions = stageOptions;

  form: FormGroup;

  appear() {
    this.buildForm();
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
  }

  save() {
    if (!this.form?.valid) {
      this.form.markAllAsTouched();
      return;
    }

    const issue = this.form.value as IssueDto;
    this.submit.emit(issue);
    this.close();
  }

  close() {
    this.visible = false;
    this.visibleChange.emit(this.visible);
  }
}
