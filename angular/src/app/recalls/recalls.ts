import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { forkJoin, of } from 'rxjs';

import { SHARED_IMPORTS } from '../shared/shared-imports.constants';
import { RecallService, RecallDto, ExternalRecallDetailDto, CreateRecallDto, UpdateRecallDto } from '../proxy/recalls';
import { RecallStatus } from '../proxy/recalls/recall-status.enum';
import { RecallType } from '../proxy/recalls/recall-type.enum';
import { ToasterHelperService } from '../shared/services/toaster-helper.service';

@Component({
  selector: 'app-recalls',
  imports: [...SHARED_IMPORTS],
  templateUrl: './recalls.html',
  styleUrl: './recalls.scss',
})
export class Recalls {
  private readonly recallService = inject(RecallService);
  private readonly fb = inject(FormBuilder);
  private readonly toaster = inject(ToasterHelperService);

  @Input() carId: string;
  @Output() submit = new EventEmitter<boolean>();

  @Input() visible: boolean = false;
  @Output() visibleChange = new EventEmitter<boolean>();

  loading = true;
  saving = false;

  mode: 'db' | 'external' = 'db';

  recalls: RecallDto[] = [];
  externalRecalls: ExternalRecallDetailDto[] = [];

  form = this.fb.group({
    dbRows: this.fb.array<FormGroup>([]),
    externalRows: this.fb.array<FormGroup>([]),
  });

  readonly RecallStatus = RecallStatus;
  readonly RecallType = RecallType;

  modalOptions = {
    size: 'lg',
    backdrop: 'static',
    keyboard: false,
    animation: true,
  };

  get dbRows(): FormArray<FormGroup> {
    return this.form.get('dbRows') as FormArray<FormGroup>;
  }

  get externalRows(): FormArray<FormGroup> {
    return this.form.get('externalRows') as FormArray<FormGroup>;
  }

  get(): void {
    this.loading = true;
    this.mode = 'db';

    // =========================================================
    // 🧪 TESTING BLOCK - REMOVE THIS WHEN DONE
    // =========================================================
    // setTimeout(() => { // Small timeout to simulate loading
    //   this.loadDummyData();
    //   this.loading = false;
    // }, 500);
    // return;
    // =========================================================

    this.recalls = [];
    this.externalRecalls = [];
    this.dbRows.clear();
    this.externalRows.clear();

    if (!this.carId) {
      this.loading = false;
      return;
    }

    this.recallService.getListByCar(this.carId).subscribe((recalls) => {
      this.recalls = recalls ?? [];

      if (this.recalls.length > 0) {
        this.mode = 'db';
        this.buildDbRows(this.recalls);
        this.loading = false;
        return;
      }

      // No DB recalls => load external
      this.mode = 'external';
      this.recallService.getRecallsFromExternalService(this.carId!).subscribe((external) => {
        this.externalRecalls = external ?? [];
        this.buildExternalRows(this.externalRecalls);
        this.loading = false;
      });
    });
  }

  private buildDbRows(items: RecallDto[]): void {

    items.sort((a, b) => (a.type === RecallType.Csps ? 1 : -1));

    for (const r of items) {
      this.dbRows.push(
        this.fb.group({
          id: [r.id, Validators.required],
          title: [r.title ?? null, [Validators.required, Validators.maxLength(256)]],

          // show only (not editable). enable later if needed
          make: [r.make],
          manufactureId: [r.manufactureId],

          riskDescription: [r.riskDescription ?? null, Validators.maxLength(4000)],
          status: [r.status ?? RecallStatus.Pending, Validators.required],
          // type: [r.type ?? RecallType.Recalls, Validators.required],
          isCsp: [r.type === RecallType.Csps],
          notes: [r.notes ?? null, Validators.maxLength(4000)],
          concurrencyStamp: [r.concurrencyStamp ?? null],

          // For Files
          tempFiles: [[]], 
          existingFiles: [r.entityAttachments ?? []]
        })
      );
    }
  }

  private buildExternalRows(items: ExternalRecallDetailDto[]): void {
    for (const x of items) {
      this.externalRows.push(
        this.fb.group({
          title: [x.title ?? null, [Validators.required, Validators.maxLength(256)]],
          make: [x.make ?? null],
          manufacturerId: [x.manufacturerId ?? null],
          riskDescription: [x.riskDescription ?? null, Validators.maxLength(4000)],
          status: [RecallStatus.Pending, Validators.required], // default Pending
          isCsp: [false],
          notes: [null, Validators.maxLength(4000)],

          // --- NEW: File Arrays ---
          tempFiles: [[]], 
          existingFiles: [[]] // External items usually start with no existing files
        })
      );
    }
  }

  save(): void {
    if (this.loading || this.saving) return;

    if (this.mode === 'db') {
      this.dbRows.markAllAsTouched();
      // ---
      // const dirtyControls = this.dbRows.controls.filter(fg => fg.dirty);
      // if (dirtyControls.length === 0) {
      //   this.close();
      //   return;
      // }
      // ---
      if (this.dbRows.invalid) return;

      const requests = this.dbRows.controls.map((fg) => {
        const v = fg.getRawValue();

        const payload: UpdateRecallDto = {
          id: v.id,
          title: v.title,
          type: v.isCsp ? RecallType.Csps : RecallType.Recalls,
          status: v.status,
          notes: v.notes ?? undefined,
          concurrencyStamp: v.concurrencyStamp,
          // --- NEW: Map files from the row ---
          tempFiles: v.tempFiles,
          entityAttachments: v.existingFiles
        };

        return this.recallService.update(v.id, payload);

      });

      if (!requests.length) {
        this.close();
        return;
      }

      this.saving = true;
      forkJoin(requests).subscribe({
        next: () => {
          this.toaster.updated();
          this.submit.emit(true);
          this.close();
        },
        error: () => {
          // keep it simple; ABP will show error toast if configured
          this.saving = false;
        },
        complete: () => {
          this.saving = false;
        },
      });

      return;
    }

    // External mode
    this.externalRows.markAllAsTouched();
    if (this.externalRows.invalid) return;
    if (!this.carId) return;

    const requests = this.externalRows.controls.map((fg) => {
      const v = fg.value;

      const payload: CreateRecallDto = {
        carId: this.carId!,
        title: v.title,
        make: v.make ?? undefined,
        manufactureId: v.manufacturerId ?? undefined,
        riskDescription: v.riskDescription ?? undefined,
        status: v.status ?? RecallStatus.Pending,
        type: v.isCsp ? RecallType.Csps : RecallType.Recalls,
        notes: v.notes ?? undefined,

        // --- NEW: Map files from the row ---
        tempFiles: v.tempFiles
      };

      return this.recallService.create(payload);
    });

    if (!requests.length) {
      this.close();
      return;
    }

    this.saving = true;
    forkJoin(requests).subscribe({
      next: () => {
        this.toaster.created();
        this.submit.emit(true);
        this.close();
      },
      error: () => {
        this.saving = false;
      },
      complete: () => {
        this.saving = false;
      },
    });
  }

  // Helper to reduce code duplication (Optional)
  private handleSaveRequests(requests: any[]) {
    if (!requests.length) { this.close(); return; }
    this.saving = true;
    forkJoin(requests).subscribe({
      next: () => {
        this.toaster.updated(); // or created
        this.submit.emit(true);
        this.close();
      },
      error: () => { this.saving = false; },
      complete: () => { this.saving = false; },
    });
  }

  close(): void {
    this.visible = false;
    this.visibleChange.emit(this.visible);
  }

  // Check if any row is a regular Recall (isCsp == false)
  hasRecalls(formArray: FormArray): boolean {
    return formArray.controls.some(group => !group.get('isCsp')?.value);
  }

  // Check if any row is a CSP (isCsp == true)
  hasCsps(formArray: FormArray): boolean {
    return formArray.controls.some(group => group.get('isCsp')?.value);
  }






  ///--------
  // private loadDummyData(): void {
  //   const dummyData: RecallDto[] = [
  //     // 1. Standard Recall (Pending)
  //     {
  //       id: '1',
  //       title: 'Loss of Steering Control due to Bolt Fracture',
  //       make: 'FORD',
  //       manufactureId: '23S01',
  //       riskDescription: 'A detached control arm can cause a loss of vehicle steering and control, increasing the risk of a crash.',
  //       status: RecallStatus.Pending,
  //       type: RecallType.Recalls, // Should be at TOP
  //       notes: null,
  //       concurrencyStamp: 'abc'
  //     } as any,

  //     // 2. CSP (Pending) - Should sort to BOTTOM
  //     {
  //       id: '2',
  //       title: 'CSP: Door Latch Freezing in Extreme Cold',
  //       make: 'FORD',
  //       manufactureId: '21N03',
  //       riskDescription: 'Doors may not latch in sub-zero temperatures.',
  //       status: RecallStatus.Pending,
  //       type: RecallType.Csps, // Should be at BOTTOM
  //       notes: 'Customer notified via mail.',
  //       concurrencyStamp: 'def'
  //     } as any,

  //     // 3. Standard Recall (Completed)
  //     {
  //       id: '3',
  //       title: 'Rear View Camera Software Update',
  //       make: 'LINCOLN',
  //       manufactureId: '22S45',
  //       riskDescription: 'Rear view camera image may not display.',
  //       status: RecallStatus.Completed,
  //       type: RecallType.Recalls, // Should be at TOP
  //       notes: 'Fixed on previous visit.',
  //       concurrencyStamp: 'ghi'
  //     } as any,

  //     // 4. Another CSP
  //     {
  //       id: '4',
  //       title: 'CSP: Wiper Motor Replacement Extended Coverage',
  //       make: 'FORD',
  //       manufactureId: '19M01',
  //       riskDescription: null,
  //       status: RecallStatus.Pending,
  //       type: RecallType.Csps, // Should be at BOTTOM
  //       notes: null,
  //       concurrencyStamp: 'jkl'
  //     } as any,
  //   ];

  //   this.recalls = dummyData;
  //   this.dbRows.clear();
  //   this.buildDbRows(this.recalls); // This will trigger the sorting logic we wrote
  // }
}
