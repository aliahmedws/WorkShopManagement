import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';

import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';

import { LocalizationModule } from '@abp/ng.core';
import { ThemeSharedModule, ConfirmationService, ToasterService } from '@abp/ng.theme.shared';

import { CarModelMake, CarModelMakeImage } from '../models/car-model-makes';
import { CarModelMakesStorage } from '../models/car-model-makes.storage';

@Component({
  selector: 'app-car-model',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NgbDropdownModule,
    NgxDatatableModule,
    LocalizationModule,
    ThemeSharedModule,
  ],
  templateUrl: './car-model.html',
  styleUrls: ['./car-model.scss'],
})
export class CarModel implements OnInit {
  all: CarModelMake[] = [];
  view: CarModelMake[] = [];

  filter = '';
  isModalOpen = false;
  saving = false;
  editingId: string | null = null;

  draftImages: CarModelMakeImage[] = [];

  readonly maxImages = 8;
  readonly maxFileSizeMB = 1.5;

  form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(128)]],
  });

  constructor(
    private readonly fb: FormBuilder,
    private readonly store: CarModelMakesStorage,
    private readonly confirmation: ConfirmationService,
    private readonly toaster: ToasterService
  ) {}

  ngOnInit(): void {
    this.store.ensureDefaults();
    this.reload();
  }

  reload(): void {
    this.all = this.store.getAll();
    this.applyFilter();
  }

  applyFilter(): void {
    const q = (this.filter ?? '').trim().toLowerCase();
    this.view = !q ? this.all : this.all.filter(x => x.name.toLowerCase().includes(q));
  }

  openCreate(): void {
    this.editingId = null;
    this.draftImages = [];
    this.form.reset({ name: '' });
    this.isModalOpen = true;
  }

  openEdit(row: CarModelMake): void {
    this.editingId = row.id;
    this.draftImages = [...(row.images ?? [])];
    this.form.reset({ name: row.name });
    this.isModalOpen = true;
  }

  removeDraftImage(imageId: string): void {
    this.draftImages = this.draftImages.filter(x => x.id !== imageId);
  }

  async onImagesSelected(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;
    const files = Array.from(input.files ?? []);
    if (!files.length) return;

    input.value = '';

    const remainingSlots = this.maxImages - this.draftImages.length;
    if (remainingSlots <= 0) {
      this.toaster.warn(`Maximum ${this.maxImages} images allowed.`);
      return;
    }

    const toAdd = files.slice(0, remainingSlots);

    for (const file of toAdd) {
      const sizeMb = file.size / (1024 * 1024);
      if (sizeMb > this.maxFileSizeMB) {
        this.toaster.warn(`"${file.name}" is too large. Max ${this.maxFileSizeMB}MB.`);
        continue;
      }

      if (!file.type.startsWith('image/')) {
        this.toaster.warn(`"${file.name}" is not an image.`);
        continue;
      }

      const dataUrl = await this.readAsDataUrl(file);
      this.draftImages.push({
        id: this.newId(),
        fileName: file.name,
        contentType: file.type,
        dataUrl,
      });
    }
  }

  save(): void {
    if (this.form.invalid) return;

    this.saving = true;

    try {
      const name = this.form.value.name ?? '';

      if (this.editingId) {
        this.store.update(this.editingId, name, this.draftImages);
        this.toaster.success('Updated successfully');
      } else {
        this.store.create(name, this.draftImages);
        this.toaster.success('Created successfully');
      }

      this.isModalOpen = false;
      this.reload();
    } catch (e: any) {
      this.toaster.error(e?.message ?? 'Operation failed');
    } finally {
      this.saving = false;
    }
  }

  delete(row: CarModelMake): void {
    this.confirmation.warn(`Delete "${row.name}"?`, 'Confirm').subscribe(status => {
      if (status !== 'confirm') return;
      this.store.delete(row.id);
      this.toaster.success('Deleted successfully');
      this.reload();
    });
  }

  clearAll(): void {
    this.confirmation.warn('This will remove all local data for Car Models.', 'Confirm').subscribe(status => {
      if (status !== 'confirm') return;
      this.store.clear();
      this.toaster.success('Local data cleared');
      this.reload();
    });
  }

  private readAsDataUrl(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onerror = () => reject(new Error('Failed to read file'));
      reader.onload = () => resolve(String(reader.result));
      reader.readAsDataURL(file);
    });
  }

  private newId(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
      const r = crypto.getRandomValues(new Uint8Array(1))[0] & 15;
      const v = c === 'x' ? r : (r & 0x3) | 0x8;
      return v.toString(16);
    });
  }

 get totalModels(): number {
  return this.all?.length ?? 0;
}

get modelsWithImages(): number {
  return (this.all ?? []).filter(m => (m.images?.length ?? 0) > 0).length;
}

get totalImages(): number {
  return (this.all ?? []).reduce((sum, m) => sum + (m.images?.length ?? 0), 0);
}

refresh(): void {
  this.reload();
}


}
