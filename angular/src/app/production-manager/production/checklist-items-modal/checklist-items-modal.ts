import { Component, EventEmitter, Output, ViewChild, inject } from '@angular/core';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { ListItemDto, ListItemService } from 'src/app/proxy/list-items';
import { finalize } from 'rxjs/operators';
import { CarBayItemService } from 'src/app/proxy/car-bay-items';
import { EntityAttachmentDto } from 'src/app/proxy/entity-attachments';
import { FileAttachmentDto } from 'src/app/proxy/entity-attachments/file-attachments';
import { FileUploadModal } from 'src/app/shared/components/file-upload-modal/file-upload-modal';

@Component({
  selector: 'app-checklist-items-modal',
  standalone: true,
  imports: [...SHARED_IMPORTS, FileUploadModal],
  templateUrl: './checklist-items-modal.html',
  styleUrls: ['./checklist-items-modal.scss'],
})
export class CheckListItemsModal {
  private readonly listItemService = inject(ListItemService);
  private readonly carBayItemService = inject(CarBayItemService);

  visible = false;
  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() saved = new EventEmitter<void>();

  @ViewChild('uploadModal') uploadModal!: FileUploadModal;

  carBayId?: string;
  checkListId?: string;
  checkListName?: string;
  activeListItemId?: string;
  activeTempFiles: FileAttachmentDto[] = [];
  activeExistingFiles: EntityAttachmentDto[] = [];

  savingRow: Record<string, boolean> = {};

  loading = false;
  saving = false;

  items: ListItemDto[] = [];

  selectedMarks: Record<string, string | null> = {};
  comments: Record<string, string> = {};

  attachmentsByListItemId: Record<string, EntityAttachmentDto[]> = {};
  tempFilesByListItemId: Record<string, FileAttachmentDto[]> = {};

  existingRowId: Record<string, string> = {};

  // ✅ Store selected browser files per listItemId (simple approach)
  selectedLocalFiles: Record<string, File[]> = {};

  // ✅ NEW: Store the single "Last Updated" info for the whole form
  lastModifiedInfo: { name: string; time: string | Date } | null = null;

  open(carBayId: string, checkListId: string, checkListName?: string): void {
    this.carBayId = carBayId;
    this.checkListId = checkListId;
    this.checkListName = checkListName ?? '';
    this.visible = true;
    this.visibleChange.emit(true);

    this.load();
  }

  close(): void {
    this.visible = false;
    this.visibleChange.emit(false);

    this.carBayId = undefined;
    this.checkListId = undefined;
    this.checkListName = undefined;

    this.items = [];
    this.selectedMarks = {};
    this.comments = {};
    this.attachmentsByListItemId = {};
    this.tempFilesByListItemId = {};
    this.existingRowId = {};
    this.selectedLocalFiles = {};
    
    // Clear status
    this.lastModifiedInfo = null;

    this.loading = false;
    this.saving = false;
  }

  canAttach(li: ListItemDto): boolean {
    return li.isSeparator !== true && li.isAttachmentRequired === true;
  }

  trackById(_: number, item: ListItemDto): string {
    return item.id!;
  }

  triggerFilePicker(inputEl: HTMLInputElement): void {
    inputEl.click();
  }

  onFilesSelected(listItemId: string, event: Event): void {
    const input = event.target as HTMLInputElement;
    const files = Array.from(input.files ?? []);

    if (files.length === 0) return;

    const existing = this.selectedLocalFiles[listItemId] ?? [];
    this.selectedLocalFiles[listItemId] = [...existing, ...files];

    input.value = '';
  }

  removeLocalFile(listItemId: string, index: number): void {
    const existing = this.selectedLocalFiles[listItemId] ?? [];
    if (index < 0 || index >= existing.length) return;

    existing.splice(index, 1);
    this.selectedLocalFiles[listItemId] = [...existing];
  }

  // --------------------
  // Load/Save Logic
  // --------------------

  private load(): void {
    if (!this.checkListId) return;

    this.loading = true;
    this.lastModifiedInfo = null; // reset before load

    this.listItemService.getByCheckListWithDetails(this.checkListId).subscribe({
      next: async (res) => {
        this.items = (res ?? []).slice().sort((a, b) => (a.position ?? 0) - (b.position ?? 0));

        for (const li of this.items) {
          if (!li?.id || li.isSeparator === true) continue;
          this.selectedMarks[li.id] ??= null;
          this.comments[li.id] ??= '';
          this.attachmentsByListItemId[li.id] ??= [];
          this.tempFilesByListItemId[li.id] ??= [];
          this.selectedLocalFiles[li.id] ??= [];
        }

        await this.preloadExistingValues();
        this.loading = false;
      },
      error: () => (this.loading = false),
    });
  }

  private async preloadExistingValues(): Promise<void> {
    if (!this.carBayId) return;

    const listItemIds = this.items
      .filter(x => x?.id && x.isSeparator !== true)
      .map(x => x.id!);

    if (listItemIds.length === 0) return;

    try {
      const res = await this.carBayItemService.getList({
        carBayId: this.carBayId,
        skipCount: 0,
        maxResultCount: 1000,
      } as any).toPromise();

      const rows = (res?.items ?? []) as any[];

      // Variables to track the latest modification
      let maxTimeValue = 0;

      for (const r of rows) {
        const listItemId = r.checkListItemId;
        if (!listItemId || !listItemIds.includes(listItemId)) continue;
        
        this.existingRowId[listItemId] = r.id;

        // 1. Map Values
        const li = this.items.find(x => x.id === listItemId);
        if (li) {
          const savedName = r.checkRadioOption as string | null;
          const mappedId = savedName
            ? (li.radioOptions ?? []).find(ro => ro.name === savedName)?.id ?? null
            : null;

          this.selectedMarks[listItemId] = mappedId;
        }

        this.comments[listItemId] = r.comments ?? '';
        this.attachmentsByListItemId[listItemId] = r.entityAttachments ?? [];

        // 2. Check for latest edit time
        const rowTime = r.lastModificationTime || r.creationTime;
        const rowName = r.modifierName || r.creatorName;

        if (rowTime) {
          const tVal = new Date(rowTime).getTime();
          if (tVal > maxTimeValue) {
             maxTimeValue = tVal;
             this.lastModifiedInfo = {
               name: rowName || 'Unknown',
               time: rowTime
             };
          }
        }
      }
    } catch {
      // ignore
    }
  }

  clearMark(listItemId: string): void {
    this.selectedMarks[listItemId] = null;
  }

  save(): void {
    if (!this.carBayId) return;

    const rows = this.items
      .filter(x => x?.id && x.isSeparator !== true)
      .map(li => {
        const listItemId = li.id!;
        const selectedRadioId = this.selectedMarks[listItemId];
        const selectedRadioName =
          selectedRadioId
            ? (li.radioOptions ?? []).find(ro => ro.id === selectedRadioId)?.name ?? null
            : null;

        return {
          id: this.existingRowId[listItemId] ?? null,
          carBayId: this.carBayId!,
          checkListItemId: listItemId,
          checkRadioOption: selectedRadioName,
          comments: (this.comments[listItemId] ?? '').trim() || null,
          tempFiles: this.tempFilesByListItemId[listItemId] ?? [],
          entityAttachments: this.attachmentsByListItemId[listItemId] ?? [],
        };
      });

    this.saving = true;

    this.carBayItemService.saveBatch({ items: rows } as any)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe({
        next: (res : any) => {
          for (const li of this.items) {
          if (!li?.id || li.isSeparator === true) continue;
          this.tempFilesByListItemId[li.id] = [];
          this.saved.emit();
          this.close();
        }
      }
    });}


onTempFilesChanged(listItemId: string, files: FileAttachmentDto[]): void {
  this.tempFilesByListItemId[listItemId] = files ?? [];
  this.updateRowAttachments(listItemId);
}

onExistingFilesChanged(listItemId: string, files: EntityAttachmentDto[]): void {
  this.attachmentsByListItemId[listItemId] = files ?? [];
  this.updateRowAttachments(listItemId);
}

private updateRowAttachments(listItemId: string): void {
  if (!this.carBayId) return;

  // prevent concurrent spam for same row
  if (this.savingRow[listItemId]) return;
  this.savingRow[listItemId] = true;

  const li = this.items.find(x => x.id === listItemId);
  if (!li) {
    this.savingRow[listItemId] = false;
    return;
  }

  const selectedRadioId = this.selectedMarks[listItemId];
  const selectedRadioName =
    selectedRadioId
      ? (li.radioOptions ?? []).find(ro => ro.id === selectedRadioId)?.name ?? null
      : null;

  const payload = {
    items: [
      {
        id: this.existingRowId[listItemId] ?? null,
        carBayId: this.carBayId,
        checkListItemId: listItemId,

        // keep existing values so update does not wipe them
        checkRadioOption: selectedRadioName,
        comments: (this.comments[listItemId] ?? '').trim() || null,

        // attachments
        tempFiles: this.tempFilesByListItemId[listItemId] ?? [],
        entityAttachments: this.attachmentsByListItemId[listItemId] ?? [],
      },
    ],
  };

  this.carBayItemService
    .saveBatch(payload as any)
    .pipe(finalize(() => (this.savingRow[listItemId] = false)))
    .subscribe({
      next: (res: any) => {
        // If backend returns created/updated row id, store it so next updates are true updates
        // Adjust this depending on your API response shape.
        const returnedId = res?.items?.[0]?.id || res?.id;
        if (returnedId) this.existingRowId[listItemId] = returnedId;
      },
      error: () => {
        // Optional: show a toast, but don't break UI
      },
    });
}

}