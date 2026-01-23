import { CommonModule } from '@angular/common';
import { booleanAttribute, Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { NzImageModule } from 'ng-zorro-antd/image';

import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { UploadFileService } from 'src/app/shared/components/file-upload/upload-files.service';

import { EntityAttachmentDto } from 'src/app/proxy/entity-attachments';
import { FileAttachmentDto } from 'src/app/proxy/entity-attachments/file-attachments';

type FileValidationError = { fileName: string; reason: string };

export type UploadContext = {
  listItemId?: string;
  carBayId?: string;
  entityId?: string;
};

@Component({
  selector: 'app-file-upload-modal',
  standalone: true,
  imports: [...SHARED_IMPORTS, CommonModule, NzImageModule],
  templateUrl: './file-upload-modal.html',
  styleUrls: ['./file-upload-modal.scss'],
})
export class FileUploadModal {
  // Modal visible (ABP modal requires it for [(visible)])
  visible = false;
  @Output() visibleChange = new EventEmitter<boolean>();

  // Modal title
  @Input() title = 'Attachments';

  // Upload behavior
  @Input({ transform: booleanAttribute }) multiple = true;
  @Input() maxSizeMB = 10;
  @Input() acceptedTypes: string[] = ['image/*', 'application/pdf'];

  // Data binding
  @Input() tempFiles: FileAttachmentDto[] = [];
  @Output() tempFilesChange = new EventEmitter<FileAttachmentDto[]>();

  @Input() existingFiles: EntityAttachmentDto[] = [];
  @Output() existingFilesChange = new EventEmitter<EntityAttachmentDto[]>();

  // Optional: context comes from parent (row/li etc.)
  @Input() context?: UploadContext;
  @Output() opened = new EventEmitter<UploadContext | undefined>();

  @Input() buttonIcon = 'fas fa-cloud-upload fa-lg';
  @Input() buttonTitle = 'Attachments';
  @Input({ transform: booleanAttribute }) disabled = false;

  @Input() modalSize: 'sm' | 'md' | 'lg' | 'xl' = 'lg';


  dragging = false;
  error = '';

  // ---------------------------
  // OPEN / CLOSE
  // ---------------------------
  open(): void {
    if (this.disabled) return;

    this.visible = true;
    this.visibleChange.emit(true);
    this.opened.emit(this.context);
  }

  close(): void {
    this.visible = false;
    this.visibleChange.emit(false);
  }

  onTempFilesChange(files: FileAttachmentDto[]): void {
    this.tempFiles = files ?? [];
    this.tempFilesChange.emit(this.tempFiles);
  }

  onExistingFilesChange(files: EntityAttachmentDto[]): void {
    this.existingFiles = files ?? [];
    this.existingFilesChange.emit(this.existingFiles);
  }
}
