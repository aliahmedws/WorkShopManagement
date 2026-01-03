import { CommonModule } from '@angular/common';
import { booleanAttribute, Component, EventEmitter, Input, Output } from '@angular/core';
import { FileAttachmentDto } from 'src/app/proxy/entity-attachments/file-attachments';

@Component({
  selector: 'app-file-upload',
  imports: [CommonModule],
  templateUrl: './file-upload.component.html',
  styleUrl: './file-upload.component.scss'
})
export class FileUploadComponent {
  @Input({ transform: booleanAttribute }) multiple: boolean = false;
  @Input() maxSizeMB: number = 10;
  @Input() acceptedTypes: string[] = ['image/*', 'application/pdf'];
  // @Output() filesSelected = new EventEmitter<File[]>();
  @Input() tempFiles: FileAttachmentDto[] = [];
  @Input() existingFiles: FileAttachmentDto[] = [];

  @Output() filesSelected = new EventEmitter<File[]>();
  @Output() removeTemp = new EventEmitter<number>();
  @Output() removeExisting = new EventEmitter<number>();

  isDragging = false;
  selectedFiles: File[] = [];
  error: string = '';

  onFileSelect(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files) {
      this.handleFiles(Array.from(input.files));
      input.value = '';
    }
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    this.isDragging = true;
  }

  onDragLeave(event: DragEvent): void {
    event.preventDefault();
    this.isDragging = false;
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    this.isDragging = false;
    if (event.dataTransfer?.files) {
      this.handleFiles(Array.from(event.dataTransfer.files));
    }
  }

  private handleFiles(files: File[]): void {
    this.error = '';
    const validFiles: File[] = [];

    for (const file of files) {
      if (!this.validateFile(file)) continue;
      validFiles.push(file);
      if (!this.multiple) break;
    }

    if (validFiles.length > 0) {
      this.selectedFiles = validFiles;
      this.filesSelected.emit(validFiles);
    }
  }

  private validateFile(file: File): boolean {
    // Check file size
    if (file.size > this.maxSizeMB * 1024 * 1024) {
      this.error = `File size exceeds ${this.maxSizeMB}MB`;
      return false;
    }

    // Check file type
    const isValidType = this.acceptedTypes.some(type => {
      if (type.endsWith('/*')) {
        return file.type.startsWith(type.replace('/*', ''));
      }
      return file.type === type;
    });

    if (!isValidType) {
      this.error = 'Invalid file type';
      return false;
    }

    return true;
  }

  removeFile(index: number): void {
    this.selectedFiles.splice(index, 1);
    this.filesSelected.emit(this.selectedFiles);
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
  }

  isImage(name?: string): boolean {
    if (!name) return false;
    return /\.(jpg|jpeg|png|gif|webp|bmp)$/i.test(name);
  }

  onRemoveTemp(i: number): void {
    this.removeTemp.emit(i);
  }

  onRemoveExisting(i: number): void {
    this.removeExisting.emit(i);
  }

  

}