import { Component, EventEmitter, Input, Output } from '@angular/core';
import { stageOptions } from 'src/app/proxy/cars/stages';
import { issueDeteriorationTypeOptions, issueOriginStageOptions, issueStatusOptions, issueTypeOptions } from 'src/app/proxy/issues';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';

@Component({
  selector: 'app-damage-marker-details',
  imports: [...SHARED_IMPORTS],
  templateUrl: './damage-marker-details.html',
  styleUrl: './damage-marker-details.scss'
})
export class DamageMarkerDetails {
  @Input() visible: boolean;
  @Output() visibleChange = new EventEmitter<boolean>();

  @Input() vin: string | null;
  @Input() issue: any = {};

  loading = false;

  issueStatusOptions = issueStatusOptions;
  issueTypeOptions = issueTypeOptions;
  issueOriginStageOptions = issueOriginStageOptions;
  issueDeteriorationTypeOptions = issueDeteriorationTypeOptions;
  stageOptions = stageOptions;

  appear() {
  }
}
