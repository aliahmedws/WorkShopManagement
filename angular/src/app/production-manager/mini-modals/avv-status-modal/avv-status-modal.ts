import { Component, EventEmitter, Input, Output, inject, OnChanges, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CarService } from 'src/app/proxy/cars'; // Adjust path if needed
import { avvStatusOptions, AvvStatus } from 'src/app/proxy/car-bays'; // Adjust path
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';

@Component({
  selector: 'app-avv-status-modal',
  standalone: true,
  imports: [...SHARED_IMPORTS, ReactiveFormsModule],
  templateUrl: './avv-status-modal.html',
  styleUrl: './avv-status-modal.scss'
})
export class AvvStatusModal implements OnChanges {
  private readonly fb = inject(FormBuilder);
  private readonly carService = inject(CarService);
  private readonly toaster = inject(ToasterHelperService);

  @Input() visible = false;
  @Output() visibleChange = new EventEmitter<boolean>();

  // Inputs for data
  @Input() carId?: string;
  @Input() vin?: string;
  @Input() currentStatus?: AvvStatus;

  // Output to refresh parent list
  @Output() statusUpdated = new EventEmitter<void>();

  form!: FormGroup;
  aVVStatusOptions = avvStatusOptions;

  ngOnChanges(changes: SimpleChanges): void {
    // Rebuild form if visible changes to true or data changes
    if (changes['visible']?.currentValue === true || changes['currentStatus']) {
      this.buildForm();
    }
  }

  private buildForm() {
    this.form = this.fb.group({
      avvStatus: [this.currentStatus ?? null, [Validators.required]],
    });
  }

  close() {
    this.visible = false;
    this.visibleChange.emit(this.visible);
  }

  save() {
    if (!this.carId || this.form.invalid) return;

    const avvStatus = this.form.value.avvStatus;

    this.carService.updateAvvStatus(this.carId, { avvStatus }).subscribe(() => {
      this.toaster.success('::AVVStatusUpdated', '::Success');
      this.statusUpdated.emit();
      this.close();
    });
  }
}