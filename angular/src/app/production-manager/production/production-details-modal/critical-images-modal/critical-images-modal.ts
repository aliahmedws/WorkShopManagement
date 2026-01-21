import { Component, EventEmitter, inject, Input, Output, SimpleChanges, OnChanges } from '@angular/core';
import { NzImageModule } from 'ng-zorro-antd/image';
import { finalize } from 'rxjs/operators';
import { CarBayService } from 'src/app/proxy/car-bays';
import { SignedHrefDirective } from 'src/app/shared/services/signed-urls/signed-href.directive';
import { SignedNzImageSrcDirective } from 'src/app/shared/services/signed-urls/signed-nz-image-src.directive';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';

@Component({
  selector: 'app-critical-images-modal',
  standalone: true,
  imports: [...SHARED_IMPORTS, NzImageModule, SignedNzImageSrcDirective],
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

   fileName(path: string): string {
    if (!path) return 'file';
    const clean = path.split('?')[0]; // remove query string if any
    const last = clean.split('/').pop() ?? 'file';
    try {
      return decodeURIComponent(last);
    } catch {
      return last;
    }
  }

  isImage(name?: string): boolean {
    if (!name) return false;
    return /\.(jpg|jpeg|png|gif|webp|bmp|svg)$/i.test(name);
  }
}
