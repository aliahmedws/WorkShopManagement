import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { EntityAttachmentDto } from 'src/app/proxy/entity-attachments';
import { FileAttachmentDto } from 'src/app/proxy/entity-attachments/file-attachments';
import { ArrivalEstimateService, ArrivalEstimateDto, UpdateArrivalEstimateDto, CreateArrivalEstimateDto } from 'src/app/proxy/logistics-details/arrival-estimates';
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { NgbDateNativeAdapter, NgbDateAdapter } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-arrival-estimates-create-edit-modal',
  standalone: true,
  imports: [...SHARED_IMPORTS],
  templateUrl: './arrival-estimates-create-edit-modal.html',
  styles: '',
  providers: [{ provide: NgbDateAdapter, useClass: NgbDateNativeAdapter }]
})
export class ArrivalEstimatesCreateEditModal {
  private readonly fb = inject(FormBuilder);
  private readonly service = inject(ArrivalEstimateService);
  private readonly toaster = inject(ToasterHelperService);

  @Input() logisticsDetailId!: string;
  @Input() estimateId?: string; // If present, Edit Mode
  @Input() visible = false;
  
  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() submit = new EventEmitter<void>();

  form!: FormGroup;
  loading = false;

  tempFiles: FileAttachmentDto[] = [];
  existingFiles: EntityAttachmentDto[] = [];

  modalOptions = {
    size: 'lg',
    backdrop: 'static',
    keyboard: false
  };

  get isEdit(): boolean {
    return !!this.estimateId;
  }

  open() {
    this.buildForm();
    if (this.isEdit) {
      this.fetchEstimate();
    }
  }

  fetchEstimate() {
    this.loading = true;
    this.service.get(this.estimateId!).subscribe(dto => {
      this.buildForm(dto);
      this.existingFiles = dto.entityAttachments || [];
      this.loading = false;
    });
  }

  buildForm(dto?: ArrivalEstimateDto) {
    this.form = this.fb.group({
      etaPort: [{ value: dto?.etaPort ? new Date(dto.etaPort) : null, disabled: this.isEdit }, [Validators.required]],
      etaScd: [{ value: dto?.etaScd ? new Date(dto.etaScd) : null, disabled: this.isEdit }, [Validators.required]],
      notes: [dto?.notes ?? null, [Validators.maxLength(2000)]]
    });
    
    if (!this.isEdit) {
        this.existingFiles = [];
        this.tempFiles = [];
    }
  }

  save() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { etaPort, etaScd, notes } = this.form.getRawValue(); // use getRawValue to get disabled fields if needed

    this.loading = true;

    if (this.isEdit) {
      // --- UPDATE (Notes Only) ---
      const input: UpdateArrivalEstimateDto = {
        notes,
        tempFiles: this.tempFiles,
        entityAttachments: this.existingFiles
      };

      this.service.update(this.estimateId!, input).subscribe(() => {
        this.handleSuccess();
      });
    } else {
      // --- CREATE ---
      const input: CreateArrivalEstimateDto = {
        logisticsDetailId: this.logisticsDetailId,
        etaPort,
        etaScd,
        notes,
        tempFiles: this.tempFiles
      };

      this.service.create(input).subscribe(() => {
        this.handleSuccess();
      });
    }
  }
  
  
  handleSuccess() {
    this.toaster.createdOrUpdated(this.isEdit ? '::SuccessfullyUpdated' : '::SuccessfullyCreated');
    this.loading = false;
    this.close();
    this.submit.emit();
  }

  close() {
    this.visible = false;
    this.tempFiles = [];
    this.existingFiles = [];
    this.visibleChange.emit(this.visible);
  }
}
