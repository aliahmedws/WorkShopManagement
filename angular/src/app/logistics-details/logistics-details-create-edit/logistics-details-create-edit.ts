import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CreStatus, creStatusOptions, Port, portOptions } from 'src/app/proxy/cars';
import { EntityAttachmentDto } from 'src/app/proxy/entity-attachments';
import { FileAttachmentDto } from 'src/app/proxy/entity-attachments/file-attachments';
import { LogisticsDetailService, LogisticsDetailDto, UpdateLogisticsDetailDto, CreateLogisticsDetailDto } from 'src/app/proxy/logistics-details';
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { NgbDateNativeAdapter, NgbDateAdapter } from '@ng-bootstrap/ng-bootstrap';
import { ArrivalEstimatesCreateEditModal } from '../arrival-estimates/arrival-estimates-create-edit-modal/arrival-estimates-create-edit-modal';
import { ArrivalEstimateDto, ArrivalEstimateService } from 'src/app/proxy/logistics-details/arrival-estimates';


@Component({
  selector: 'app-logistics-details-create-edit',
  standalone: true,
  imports: [...SHARED_IMPORTS, ArrivalEstimatesCreateEditModal],
  templateUrl: './logistics-details-create-edit.html',
  styleUrl: './logistics-details-create-edit.scss',
  providers: [{ provide: NgbDateAdapter, useClass: NgbDateNativeAdapter }]
})
export class LogisticsDetailsCreateEdit implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly service = inject(LogisticsDetailService);
  private readonly toaster = inject(ToasterHelperService);
  private readonly estimateService = inject(ArrivalEstimateService);

  form!: FormGroup;
  carId: string | null = null;
  selectedId: string | null = null; // The LogisticsDetail ID

  // State
  loading = true;
  isEditMode = false;

  // Enums
  portOptions = portOptions;
  creStatusOptions = creStatusOptions;

  // Attachments
  tempFiles: FileAttachmentDto[] = [];
  existingFiles: EntityAttachmentDto[] = [];
  vin: string | null = null;

  // Estimate State
  latestEstimate: ArrivalEstimateDto | null = null;
  isEstimateModalVisible = false;

  logisticsDetail = {} as LogisticsDetailDto;

  ngOnInit(): void {
    this.carId = this.route.snapshot.queryParamMap.get('carId');
    this.vin = this.route.snapshot.queryParamMap.get('vin');
    if (!this.carId) {
      this.router.navigate(['/cars']);
      return;
    }

    this.fetchLogistics();
  }

  fetchLogistics() {
    this.loading = true;

    this.service.getByCarId(this.carId!).subscribe((dto) => {
      if (dto) {
        // --- EDIT MODE ---
        this.isEditMode = true;
        this.selectedId = dto.id;
        this.existingFiles = dto.entityAttachments || [];
        this.logisticsDetail = dto;
        this.buildForm(dto);

        this.fetchLatestEstimate(dto.id);
      } else {
        // --- CREATE MODE ---
        this.isEditMode = false;
        this.selectedId = null;
        this.existingFiles = [];
        this.buildForm();
      }
      this.loading = false;
    });
  }

  fetchLatestEstimate(logisticsId: string) {
    this.estimateService.getLatest(logisticsId).subscribe(res => {
      this.latestEstimate = res || null;
    });
  }

  buildForm(dto?: LogisticsDetailDto) {
    this.form = this.fb.group({
      // --- Create Fields ---
      port: [dto?.port ?? Port.Bne, [Validators.required]],
      bookingNumber: [dto?.bookingNumber ?? null, [Validators.maxLength(64)]],

      // --- Update Fields ---
      creStatus: [dto?.creStatus ?? CreStatus.Pending],
      rsvaNumber: [dto?.rsvaNumber ?? null, [Validators.maxLength(64)]],
      creSubmissionDate: [dto?.creSubmissionDate ? new Date(dto.creSubmissionDate) : null],

      clearingAgent: [dto?.clearingAgent ?? null, [Validators.maxLength(128)]],
      clearanceRemarks: [dto?.clearanceRemarks ?? null, [Validators.maxLength(500)]],
      clearanceDate: [dto?.clearanceDate ? new Date(dto.clearanceDate) : null],

      actualPortArrivalDate: [dto?.actualPortArrivalDate ? new Date(dto.actualPortArrivalDate) : null],
      actualScdArrivalDate: [dto?.actualScdArrivalDate ? new Date(dto.actualScdArrivalDate) : null],
    });

    this.tempFiles = [];
  }

  save() {
    if (this.form.invalid || !this.carId) {
      this.form.markAllAsTouched();
      return;
    }

    const formValue = this.form.value;

    if (this.isEditMode && this.selectedId) {
      // --- UPDATE FLOW ---
      const input: UpdateLogisticsDetailDto = {
        carId: this.carId,
        ...formValue,
        tempFiles: this.tempFiles,
        entityAttachments: this.existingFiles
      };

      this.service.update(this.selectedId, input).subscribe(this.handleSuccess);

    } else {
      // --- CREATE FLOW ---
      const input: CreateLogisticsDetailDto = {
        carId: this.carId,
        port: formValue.port,
        bookingNumber: formValue.bookingNumber,
        tempFiles: this.tempFiles
      };

      this.service.create(input).subscribe(this.handleSuccess);
    }
  }

  // Reusable Success Handler
  handleSuccess = () => {
    // Checks selectedId: if null => "Created", if exists => "Updated"
    this.toaster.createdOrUpdated(this.selectedId);
    this.fetchLogistics(); // Refresh state
  };

  goBack() {
    const snapshot = this.route.snapshot.queryParams;
    this.router.navigate(snapshot?.returnUrl ? [snapshot?.returnUrl] : ['/cars'], { queryParams: { 'tab': snapshot?.tab || 0 } });
  }

  addFirstEstimate() {
    this.isEstimateModalVisible = true;
  }

  viewEstimateHistory() {
    this.router.navigate(['/arrival-estimates'], {
      queryParams: {
        logisticsDetailId: this.selectedId,
        carId: this.carId // Pass carId to allow "Back" functionality
      },
      queryParamsHandling: 'merge'
    });
  }
}