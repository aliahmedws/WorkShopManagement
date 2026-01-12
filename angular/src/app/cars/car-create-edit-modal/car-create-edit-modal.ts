import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CarService, CarDto, ExternalCarDetailsDto, UpdateCarDto, CreateCarDto } from 'src/app/proxy/cars';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { NgbDateNativeAdapter, NgbDateAdapter } from '@ng-bootstrap/ng-bootstrap';
import { GuidLookupDto, LookupService } from 'src/app/proxy/lookups';
import { ToasterHelperService } from 'src/app/shared/services/toaster-helper.service';
import { storageLocationOptions } from 'src/app/proxy/cars/storage-locations';
import { Stage, stageOptions } from 'src/app/proxy/cars/stages';
import { FileAttachmentDto } from 'src/app/proxy/entity-attachments/file-attachments';
import { EntityAttachmentDto } from 'src/app/proxy/entity-attachments/models';
import { avvStatusOptions } from 'src/app/proxy/car-bays/avv-status.enum';

@Component({
  selector: 'app-car-create-edit-modal',
  imports: [...SHARED_IMPORTS],
  templateUrl: './car-create-edit-modal.html',
  styleUrl: './car-create-edit-modal.scss',
  providers: [{ provide: NgbDateAdapter, useClass: NgbDateNativeAdapter }]
})
export class CarCreateEditModal {
  private readonly fb = inject(FormBuilder);
  private readonly carService = inject(CarService);
  private readonly lookupService = inject(LookupService);
  private readonly toaster = inject(ToasterHelperService);

  @Input() carId?: string;
  @Output() submit = new EventEmitter<CarDto>();

  @Input() visible: boolean = false;
  @Output() visibleChange = new EventEmitter<boolean>();

  carModelOptions: GuidLookupDto[] = [];
  // stageOptions = stageOptions; 
  // FILTERED OPTIONS: Remove 3 and 4
  stageOptions = stageOptions.filter(x =>x.value !== 3 && x.value !== 4 );
  storageLocationOptions = storageLocationOptions;
  avvStatusOptions = avvStatusOptions;

  // Attachment State
  tempFiles: FileAttachmentDto[] = [];
  existingFiles: EntityAttachmentDto[] = [];

  carOwnerOptions: GuidLookupDto[] = [];

  form: FormGroup;

  NOT_FOUND = 'NotFound';

  modalOptions = {
    size: 'lg',
    backdrop: 'static', //prevent close by outside click
    keyboard: false, //prevent close by esc key
    animation: true,
  };

  loading: boolean = true;

  external: ExternalCarDetailsDto;

  get canFetchVinDetails(): boolean {
    const ctrl = this.form.get('vin');
    return ctrl?.value && ctrl?.valid;
  }

  get() {
    this.external = null;
    this.loading = true;

    this.resolveLookups();

    if (!this.carId) {
      this.buildForm();
      this.loading = false;
      return;
    }

    this.carService
      .get(this.carId)
      .subscribe((dto: CarDto) => {
        this.buildForm(dto);
        this.loading = false;
      });
  }

  buildForm(dto?: CarDto) {
    this.form = this.fb.group({
      vin: [dto?.vin ?? null, [Validators.required, Validators.minLength(17), Validators.maxLength(17)]],
      color: [dto?.color ?? null, [Validators.required, Validators.maxLength(64)]],
      modelId: [dto?.modelId ?? null, [Validators.required]],
      modelYear: [dto?.modelYear, [Validators.required, Validators.min(1886), Validators.max(2100)]],

      cnc: [dto?.cnc ?? null, Validators.maxLength(64)],
      cncFirewall: [dto?.cncFirewall ?? null, Validators.maxLength(64)],
      cncColumn: [dto?.cncColumn ?? null, Validators.maxLength(64)],

      dueDate: [dto?.dueDate ? new Date(dto?.dueDate) : null],
      deliverDate: [dto?.deliverDate ? new Date(dto?.deliverDate) : null],
      startDate: [dto?.startDate ? new Date(dto?.startDate) : null],

      notes: [dto?.notes ?? null, Validators.maxLength(4000)],
      missingParts: [dto?.missingParts ?? null, Validators.maxLength(4000)],


      storageLocation: [dto?.storageLocation ?? null, Validators.maxLength(128)],

      // NEW FIELDS
      buildMaterialNumber: [dto?.buildMaterialNumber ?? null, Validators.maxLength(64)],
      angleBailment: [dto?.angleBailment ?? null],
      avvStatus: [dto?.avvStatus ?? null],
      pdiStatus: [dto?.pdiStatus ?? null, Validators.maxLength(64)],

      // Stage: Keep for Edit, but we will strip it for Create in save()
      stage: [dto?.stage ?? Stage.Incoming],


      // owner
      ownerId: [dto?.ownerId ?? null, Validators.required],
      owner: this.fb.group({
        name: [''],
        email: ['', Validators.maxLength(256)],
        contactId: ['', Validators.maxLength(64)],
      }),
    });

    // Initialize existing attachments if editing
    this.existingFiles = dto?.entityAttachments || [];
    this.tempFiles = []; // Reset new uploads

    this.addListeners();
  }

  addListeners() {
    const form = this.form;
    if (!form) return;

    const ownerIdCtrl = form.get('ownerId');
    const ownerGrp = form.get('owner') as any;

    const nameCtrl = ownerGrp?.get('name');

    if (!ownerIdCtrl || !nameCtrl) return;

    // apply once for initial value
    const applyNameValidation = (ownerId: any) => {
      if (ownerId === this.NOT_FOUND) {
        nameCtrl.setValidators([Validators.required, Validators.maxLength(128)]);
      } else {
        nameCtrl.clearValidators();
      }

      nameCtrl.updateValueAndValidity({ emitEvent: false });
    };

    applyNameValidation(ownerIdCtrl.value);

    ownerIdCtrl.valueChanges.subscribe(ownerId => {
      applyNameValidation(ownerId);
    });
  }

  save(): void {
    if (this.form.invalid) {
      this.scrollToTop();
      this.form.markAllAsTouched();
      return;
    }

    const formValue = this.form.value;
    const isNewOwner = formValue.ownerId === this.NOT_FOUND;

    // Base payload
    const basePayload = {
      ...formValue,
      ownerId: isNewOwner ? null : formValue.ownerId,
      owner: isNewOwner ? formValue.owner : null,
    };

    // const req$ = this.carId
    //   ? this.carService.update(this.carId, payload)
    //   : this.carService.create(payload);

    // req$
    //   .subscribe((dto: CarDto) => {
    //     this.toaster.createdOrUpdated(this.carId);
    //     this.close();
    //     this.submit.emit(dto);
    //   });

    if (this.carId) {
      // --- UPDATE FLOW ---
      const updateInput: UpdateCarDto = {
        ...basePayload,
        tempFiles: this.tempFiles,           // New uploads
        entityAttachments: this.existingFiles // Persist/Delete existing
      };

      this.carService.update(this.carId, updateInput).subscribe(this.handleSuccess);
    } else {
      // --- CREATE FLOW ---
      // Remove Stage (backend defaults to Incoming)
      const { stage, ...createData } = basePayload;

      const createInput: CreateCarDto = {
        ...createData,
        tempFiles: this.tempFiles // New uploads only
      };

      this.carService.create(createInput).subscribe(this.handleSuccess);
    }
  }

  close(): void {
    this.visible = false;
    this.visibleChange.emit(this.visible);
  }

  resolveLookups() {
    if (!this.carModelOptions?.length) {
      this.lookupService
        .getCarModels()
        .subscribe(response => {
          this.carModelOptions = response;
        });
    }

    this.lookupService
      .getCarOwners()
      .subscribe(response => {
        this.carOwnerOptions = response;
      });
  }

  scrollToTop() {
    document.getElementById('top').scrollIntoView({ behavior: 'smooth' });
  }

  getExternalCarDetails() {
    if (!this.canFetchVinDetails) return;

    const { vin, modelYear } = this.form.value || {};
    this.carService
      .getExternalCarDetails(vin, modelYear)
      .subscribe(response => {
        this.external = response;
        this.resolveExternalCarResponse(response);
      });

  }

  resolveExternalCarResponse(response: ExternalCarDetailsDto | null) {
    if (!response?.success) {
      return;
    }

    this.form.patchValue({
      modelYear: response.modelYear
    });
  }

  // Helper to reduce code duplication
  handleSuccess = (dto: CarDto) => {
    this.toaster.createdOrUpdated(this.carId);
    this.close();
    this.submit.emit(dto);
  };
}