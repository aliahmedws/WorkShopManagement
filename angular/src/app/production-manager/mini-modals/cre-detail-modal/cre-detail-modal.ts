import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CreStatus, creStatusOptions } from 'src/app/proxy/cars';
import { CreDetailDto, LogisticsDetailService } from 'src/app/proxy/logistics-details';
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { NgbDateNativeAdapter, NgbDateAdapter } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-cre-detail-modal',
  imports: [...SHARED_IMPORTS],
  templateUrl: './cre-detail-modal.html',
  styleUrl: './cre-detail-modal.scss',
  providers: [{ provide: NgbDateAdapter, useClass: NgbDateNativeAdapter }]

})
export class CreDetailModal {

  @Input() carId: string | null;
  @Input() vin: string | null;  // to display vin in modal header
  @Input() visible: boolean;
  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() submit = new EventEmitter<void>();

  private readonly logisticsService = inject(LogisticsDetailService);
  private readonly fb = inject(FormBuilder);
  private readonly toaster = inject(ToasterHelperService);

  form: FormGroup;
  creStatusOptions = creStatusOptions;

  get() {
    this.buildForm();

    if (this.carId) {
      this.logisticsService.getCreDetailByCarId(this.carId).subscribe((res) => {
          this.buildForm(res);
      });
    }

  }

  buildForm(dto?: CreDetailDto){
    this.form = this.fb.group({
      creStatus: [dto?.creStatus ?? CreStatus.Pending],
      creSubmissionDate: [dto?.creSubmissionDate ? new Date(dto?.creSubmissionDate) : null],
      rvsaNumber: [dto?.rvsaNumber ?? null, [Validators.maxLength(60)]]
    });
  }

  close(){
    this.visible = false;
    this.visibleChange.emit(this.visible);
  }

  save(){
    if(!this.carId || this.form.invalid) return;
    const input = this.form.value;
    this.logisticsService.addOrUpdateCreDetail(this.carId, input).subscribe(() => {
      this.toaster.success('::CreDetailsUpdated', '::Success');
      this.submit.emit();
      this.close();
    });
  }

}
