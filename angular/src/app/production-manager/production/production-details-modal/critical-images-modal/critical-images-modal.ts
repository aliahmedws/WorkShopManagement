import { Component, EventEmitter, inject, Input, Output, SimpleChanges, OnChanges } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { CarBayItemService } from 'src/app/proxy/car-bay-items';
import { CarBayService } from 'src/app/proxy/car-bays';
import { CheckListDto } from 'src/app/proxy/check-lists';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';

@Component({
  selector: 'app-critical-images-modal',
  standalone: true,
  imports: [...SHARED_IMPORTS],
  templateUrl: './critical-images-modal.html',
  styleUrl: './critical-images-modal.scss',
})
export class CriticalImagesModal implements OnChanges {
  private service = inject(CarBayService);

  @Input() visible = false;
  @Output() visibleChange = new EventEmitter<boolean>();

  @Input() carBayId?: string;

  loading = false;
  images: string[] = [];

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['visible']?.currentValue === true) {
      this.load();
    }
  }

  close(): void {
    this.visibleChange.emit(false);
  }

  private load(): void {
    const carBayId = this.carBayId;
    if (!carBayId) {
      this.images = [];
      return;
    }

    this.loading = true;

    this.service.getCarBayItemImages
      (carBayId)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: res => (this.images = res ?? []),
        error: () => (this.images = []),
      });
  }

  // set the real url field your EntityAttachmentDto exposes
  getImageSrc(a: any): string {
    return a.url || a.downloadUrl || a.fileUrl || a.path || '';
  }
}
