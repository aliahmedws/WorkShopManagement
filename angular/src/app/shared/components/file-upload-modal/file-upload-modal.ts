import { CommonModule } from '@angular/common';
import { booleanAttribute, Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { NzImageModule } from 'ng-zorro-antd/image';

import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { UploadFileService } from 'src/app/shared/components/file-upload/upload-files.service';

import { EntityAttachmentDto } from 'src/app/proxy/entity-attachments';
import { FileAttachmentDto } from 'src/app/proxy/entity-attachments/file-attachments';

type FileValidationError = { fileName: string; reason: string };

@Component({
  selector: 'app-file-upload-modal',
  standalone: true,
  imports: [...SHARED_IMPORTS, CommonModule, NzImageModule],
  templateUrl: './file-upload-modal.html',
  styleUrls: ['./file-upload-modal.scss'],
})
export class FileUploadModal {
  // ✅ Required for [(visible)]
  visible = false;
  @Output() visibleChange = new EventEmitter<boolean>();

  // ✅ Modal title (optional)
  title = 'Attachments';

  // ✅ Inputs
  @Input({ transform: booleanAttribute }) multiple = true;
  @Input() maxSizeMB = 10;
  @Input() acceptedTypes: string[] = ['image/*', 'application/pdf'];

  @Input() tempFiles: FileAttachmentDto[] = [];
  @Output() tempFilesChange = new EventEmitter<FileAttachmentDto[]>();

  @Input() existingFiles: EntityAttachmentDto[] = [];
  @Output() existingFilesChange = new EventEmitter<EntityAttachmentDto[]>();

  @Input() size: null | undefined | 'small' | 'large' = null;

  private readonly uploadService = inject(UploadFileService);

  dragging = false;
  error = '';

  // ✅ Optional: for parent to know which row opened it
  context?: { listItemId?: string; carBayId?: string; entityId?: string };

  // ---------------------------
  // OPEN / CLOSE
  // ---------------------------
  open(options?: {
    title?: string;
    multiple?: boolean;
    tempFiles?: FileAttachmentDto[];
    existingFiles?: EntityAttachmentDto[];
    context?: { listItemId?: string; carBayId?: string; entityId?: string };
  }): void {
    this.error = '';
    this.dragging = false;

    if (options?.title) this.title = options.title;
    if (typeof options?.multiple === 'boolean') this.multiple = options.multiple;

    if (options?.tempFiles) this.tempFiles = options.tempFiles;
    if (options?.existingFiles) this.existingFiles = options.existingFiles;

    this.context = options?.context;

    this.visible = true;
    this.visibleChange.emit(true);
  }

  close(): void {
    this.visible = false;
    this.visibleChange.emit(false);
  }

  // ---------------------------
  // DRAG/DROP + PICK
  // ---------------------------
  triggerBrowse(input: HTMLInputElement): void {
    input.click();
  }

  onFilePicked(event: Event): void {
    const input = event.target as HTMLInputElement;
    const files = Array.from(input.files ?? []);
    input.value = '';
    if (files.length) this.handleFiles(files);
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    this.dragging = true;
  }

  onDragLeave(event: DragEvent): void {
    event.preventDefault();
    this.dragging = false;
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    this.dragging = false;

    const files = Array.from(event.dataTransfer?.files ?? []);
    if (files.length) this.handleFiles(files);
  }

  private handleFiles(files: File[]): void {
    this.error = '';

    const { validFiles, errors } = this.validateFiles(files);

    if (validFiles.length === 0) {
      if (errors.length) this.error = this.buildErrorSummary(errors);
      return;
    }

    // If some invalid files exist, show summary but still upload valid
    if (errors.length) {
      this.error = this.buildErrorSummary(errors);
    }

    const formData = new FormData();
    for (const f of validFiles) formData.append('files', f);

    this.uploadService.uploadFile(formData).subscribe({
      next: (res: FileAttachmentDto[]) => {
        this.tempFiles = [...(this.tempFiles ?? []), ...(res ?? [])];
        this.tempFilesChange.emit(this.tempFiles);
      },
      error: () => {
        this.error = 'Upload failed. Please try again.';
      },
    });
  }

  private validateFiles(files: File[]): { validFiles: File[]; errors: FileValidationError[] } {
    const errors: FileValidationError[] = [];
    const valid: File[] = [];
    const maxBytes = this.maxSizeMB * 1024 * 1024;

    for (const file of files) {
      if (file.size > maxBytes) {
        errors.push({ fileName: file.name, reason: `Too large (max ${this.maxSizeMB}MB)` });
        continue;
      }

      if (!this.isValidType(file)) {
        errors.push({ fileName: file.name, reason: 'Invalid file type' });
        continue;
      }

      valid.push(file);

      // Single mode: accept only first valid
      if (!this.multiple) break;
    }

    return { validFiles: valid, errors };
  }

  private isValidType(file: File): boolean {
    return this.acceptedTypes.some(type => {
      if (type.endsWith('/*')) {
        const prefix = type.replace('/*', '');
        return (file.type ?? '').startsWith(prefix);
      }
      return file.type === type;
    });
  }

  private buildErrorSummary(errors: FileValidationError[]): string {
    const maxList = 3;
    const shown = errors.slice(0, maxList).map(e => `${e.fileName} (${e.reason})`);
    const more = errors.length > maxList ? ` +${errors.length - maxList} more` : '';
    return `${errors.length} file(s) skipped: ${shown.join(', ')}${more}`;
  }

  // ---------------------------
  // REMOVE
  // ---------------------------
  onRemoveTemp(i: number): void {
    const list = [...(this.tempFiles ?? [])];
    list.splice(i, 1);
    this.tempFiles = list;
    this.tempFilesChange.emit(this.tempFiles);
  }

  onRemoveExisting(i: number): void {
    const list = [...(this.existingFiles ?? [])];
    list.splice(i, 1);
    this.existingFiles = list;
    this.existingFilesChange.emit(this.existingFiles);
  }

  // ---------------------------
  // HELPERS
  // ---------------------------
  isImage(name?: string): boolean {
    if (!name) return false;
    return /\.(jpg|jpeg|png|gif|webp|bmp|svg)$/i.test(name);
  }
}
