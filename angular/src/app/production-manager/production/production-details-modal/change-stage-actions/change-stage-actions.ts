import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-change-stage-actions',
  imports: [],
  templateUrl: './change-stage-actions.html',
  styleUrl: './change-stage-actions.scss'
})
export class ChangeStageActions {


  @Input() stage = null;
  @Input() carId = null;
}
