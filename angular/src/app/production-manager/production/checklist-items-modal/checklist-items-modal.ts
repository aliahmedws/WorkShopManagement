import { Component, EventEmitter, Output, inject } from '@angular/core';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { ListItemDto, ListItemService } from 'src/app/proxy/list-items';
import { finalize } from 'rxjs/operators';
import { CarBayItemService } from 'src/app/proxy/car-bay-items';
import { EntityAttachmentDto } from 'src/app/proxy/entity-attachments';
import { FileAttachmentDto } from 'src/app/proxy/entity-attachments/file-attachments';

@Component({
  selector: 'app-checklist-items-modal',
  standalone: true,
  imports: [...SHARED_IMPORTS],
  templateUrl: './checklist-items-modal.html',
  styleUrls: ['./checklist-items-modal.scss'],
})
export class CheckListItemsModal {
  private readonly listItemService = inject(ListItemService);
  private readonly carBayItemService = inject(CarBayItemService);

  visible = false;
  @Output() visibleChange = new EventEmitter<boolean>();

  carBayId?: string;
  checkListId?: string;
  checkListName?: string;

  loading = false;
  saving = false;

  items: ListItemDto[] = [];

  selectedMarks: Record<string, string | null> = {};
  comments: Record<string, string> = {};

  attachmentsByListItemId: Record<string, EntityAttachmentDto[]> = {};
  tempFilesByListItemId: Record<string, FileAttachmentDto[]> = {};

  existingRowId: Record<string, string> = {};

  // ✅ NEW: store selected browser files per listItemId (simple approach)
  selectedLocalFiles: Record<string, File[]> = {};

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

    // ✅ NEW: clear local file selection
    this.selectedLocalFiles = {};

    this.loading = false;
    this.saving = false;
  }

  canAttach(li: ListItemDto): boolean {
    return li.isSeparator !== true && li.isAttachmentRequired === true;
  }

  trackById(_: number, item: ListItemDto): string {
    return item.id!;
  }

  // ✅ STEP A: click "+" => open hidden file input
  triggerFilePicker(inputEl: HTMLInputElement): void {
    inputEl.click();
  }

  // ✅ STEP B: on selecting files => store them per listItemId
  onFilesSelected(listItemId: string, event: Event): void {
    const input = event.target as HTMLInputElement;
    const files = Array.from(input.files ?? []);

    if (files.length === 0) return;

    const existing = this.selectedLocalFiles[listItemId] ?? [];
    this.selectedLocalFiles[listItemId] = [...existing, ...files];

    // Important: reset input so selecting same file again works
    input.value = '';
  }

  // ✅ Optional: remove a selected file from UI
  removeLocalFile(listItemId: string, index: number): void {
    const existing = this.selectedLocalFiles[listItemId] ?? [];
    if (index < 0 || index >= existing.length) return;

    existing.splice(index, 1);
    this.selectedLocalFiles[listItemId] = [...existing];
  }

  // --------------------
  // Your existing load/save logic stays
  // --------------------

  private load(): void {
    if (!this.checkListId) return;

    this.loading = true;

    this.listItemService.getByCheckListWithDetails(this.checkListId).subscribe({
      next: async (res) => {
        this.items = (res ?? []).slice().sort((a, b) => (a.position ?? 0) - (b.position ?? 0));

        for (const li of this.items) {
          if (!li?.id || li.isSeparator === true) continue;
          this.selectedMarks[li.id] ??= null;
          this.comments[li.id] ??= '';
          this.attachmentsByListItemId[li.id] ??= [];
          this.tempFilesByListItemId[li.id] ??= [];
          this.selectedLocalFiles[li.id] ??= []; // ✅ NEW init
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

      for (const r of rows) {
        const listItemId = r.checkListItemId;
        if (!listItemId || !listItemIds.includes(listItemId)) continue;

        this.existingRowId[listItemId] = r.id;

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

    // NOTE: local files are currently NOT uploaded; we only show them in UI.
    // Later: upload them -> convert to tempFilesByListItemId.

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
        next: () => this.close(),
      });
  }
}
