import { Component, EventEmitter, Output, inject } from '@angular/core';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { ListItemDto, ListItemService } from 'src/app/proxy/list-items';

@Component({
  selector: 'app-checklist-items-modal',
  standalone: true,
  imports: [...SHARED_IMPORTS],
  templateUrl: './checklist-items-modal.html',
  styleUrls: ['./checklist-items-modal.scss'],
})
export class CheckListItemsModal {
  private readonly service = inject(ListItemService);

  visible = false;
  @Output() visibleChange = new EventEmitter<boolean>();

  checkListId?: string;
  checkListName?: string;

  loading = false;
  items: ListItemDto[] = [];

  /**
   * selectedMarks[listItemId] = radioOptionId
   * (you can store radioOption name if you want, but id is best)
   */
  selectedMarks: Record<string, string | null> = {};

  /**
   * comments[listItemId] = typed comment text
   */
  comments: Record<string, string> = {};

  open(checkListId: string, checkListName?: string): void {
    this.checkListId = checkListId;
    this.checkListName = checkListName ?? '';
    this.visible = true;
    this.visibleChange.emit(true);

    this.load();
  }

  close(): void {
    this.visible = false;
    this.visibleChange.emit(false);

    this.checkListId = undefined;
    this.checkListName = undefined;
    this.items = [];
    this.selectedMarks = {};
    this.comments = {};
    this.loading = false;
  }

  private load(): void {
    if (!this.checkListId) return;

    this.loading = true;

    this.service.getByCheckListWithDetails(this.checkListId).subscribe({
      next: (res) => {
        // Ensure ordered by position just in case backend doesn’t guarantee ordering
        this.items = (res ?? []).slice().sort((a, b) => (a.position ?? 0) - (b.position ?? 0));

        // Initialize comment placeholders (optional)
        for (const li of this.items) {
          if (!li?.id) continue;
          if (this.comments[li.id] === undefined) this.comments[li.id] = '';
          if (this.selectedMarks[li.id] === undefined) this.selectedMarks[li.id] = null;
        }

        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  clearMark(listItemId: string): void {
    this.selectedMarks[listItemId] = null;
  }

  canAttach(li: ListItemDto): boolean {
    return li.isSeparator !== true && li.isAttachmentRequired === true;
  }

  trackById(_: number, item: ListItemDto): string {
    return item.id!;
  }
}
