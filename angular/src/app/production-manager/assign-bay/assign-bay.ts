import { Confirmation } from '@abp/ng.theme.shared';
import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { priorityOptions, Priority, CreateCarBayDto, CarBayService } from 'src/app/proxy/car-bays';
import { GuidLookupDto, LookupService } from 'src/app/proxy/lookups';
import { StageDto } from 'src/app/proxy/stages';
import { ConfirmationHelperService } from 'src/app/shared/services/confirmation-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';

@Component({
  selector: 'app-assign-bay',
  imports: [...SHARED_IMPORTS],
  templateUrl: './assign-bay.html',
  styleUrl: './assign-bay.scss',
})
export class AssignBay {
  private lookupService = inject(LookupService);
  private fb = inject(FormBuilder);
  private confirm = inject(ConfirmationHelperService);
  private carBayService = inject(CarBayService);

  @Input() visible: boolean;
  @Output() visibleChange = new EventEmitter<boolean>();

  @Output() submit = new EventEmitter<void>();

  form!: FormGroup;
  priorityOptions = priorityOptions;
  priority = Priority;
  bayOptions: GuidLookupDto[] = [];

  @Input() selected: StageDto = {};

  appear() {
    if (!this.bayOptions?.length) this.loadBays();
    this.buildForm();
  }

  loadBays() {
    if (!this.bayOptions.length) {
      this.lookupService.getBays().subscribe(res => {
        this.bayOptions = res;
      });
    }
  }

  private buildForm(): void {
    this.form = this.fb.group({
      bayId: [this.selected.bayId || null, [Validators.required]],
      priority: [this.selected.priority || Priority.Medium, [Validators.required]],
    });
  }

  assignToBay(): void {
    if (!this.selected.carId) return;

    this.form.markAllAsTouched();
    if (this.form.invalid) return;

    this.confirm
      .confirmAction('::ConfirmAssignToBayMessage', '::ConfirmAssignToBayTitle')
      .subscribe(status => {
        if (status !== Confirmation.Status.confirm) return;

        const { bayId, priority } = this.form.value;

        const input: CreateCarBayDto = {
          carId: this.selected.carId!,
          bayId,
          priority,
          isActive: true,
        };

        this.carBayService.create(input).subscribe(() => {
          this.submit.emit();
          this.close();
        });
      });
  }

  close() {
    this.visible = false;
    this.visibleChange.emit(false);
  }
}
