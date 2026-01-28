import { Component, EventEmitter, inject, Output } from '@angular/core';
import { SpecsResponseDto } from 'src/app/proxy/external/cars-xe';
import { LookupService } from 'src/app/proxy/lookups';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';

// Interface for UI Rendering
interface DisplayItem {
  isHeader: boolean;
  key?: string;
  value?: string;
  isFullWidth?: boolean;
  isValueOnly?: boolean; // Add this new property
}

@Component({
  selector: 'app-car-external-response-modal',
  imports: [...SHARED_IMPORTS],
  templateUrl: './car-external-response-modal.html',
  styleUrl: './car-external-response-modal.scss'
})
export class CarExternalResponseModal {

  visible: boolean = false;
  @Output() visibleChange = new EventEmitter<boolean>();

  private readonly lookupService = inject(LookupService);

  specs: SpecsResponseDto | null = null;
  displayItems: DisplayItem[] = []; // Normalized data for UI
  vinNo: string = '';
  loading = false;

  modalOptions = {
    size: 'xl', // Increased size for the table layout
    keyboard: false,
    animation: true,
  };

  open(vin: string) {
    if (!vin) return;

    this.vinNo = vin;
    this.visible = true;
    this.loading = true;
    this.specs = null;
    this.displayItems = [];

    this.lookupService.getExternalSpecsResponse(vin).subscribe((res) => {
      this.specs = res;
      this.loading = false;
      if (res && res.success) {
        this.normalizeData(res);
      }
    });
  }

  close(): void {
    this.visible = false;
    this.visibleChange.emit(this.visible);
  }

  private normalizeData(data: SpecsResponseDto) {
    const items: DisplayItem[] = [];

    const formatKey = (str: string) => {
      if (!str) return '';
      return str.replace(/_/g, ' ').replace(/\b\w/g, l => l.toUpperCase());
    };

    // Helper to push items with auto-width detection
    const pushItem = (key: string, val: any, isHeader = false, forceFullWidth = false) => {
      if (isHeader) {
        items.push({ isHeader: true, value: key });
      } else {
        const valStr = String(val);
        const isFullWidth = forceFullWidth || valStr.length > 60;
        items.push({
          isHeader: false,
          key: formatKey(key),
          value: valStr,
          isFullWidth: isFullWidth
        });
      }
    };

    // Helper to process sections
    const processSection = (title: string, obj: any) => {
      if (!obj) return;
      const entries = Object.entries(obj).filter(([_, v]) => v !== null && v !== undefined && v !== '' && typeof v !== 'object');

      if (entries.length > 0) {
        pushItem(title, null, true, false); // Header
        entries.forEach(([k, v]) => pushItem(k, v, false, false));
      }
    };

    // 1. Attributes
    let exteriorColorValue: string | null = null;

    if (data.attributes) {
      pushItem('Vehicle Attributes', null, true, false);

      Object.entries(data.attributes).forEach(([key, val]) => {
        if (val === null || val === undefined || val === '') return;

        // Special handling for exterior_color - save it for later
        if (key.toLowerCase().includes('exterior') && key.toLowerCase().includes('color')) {
          if (Array.isArray(val) && val.length > 0) {
            exteriorColorValue = val.join(', ');
          } else if (typeof val === 'string') {
            exteriorColorValue = val;
          }
          return; // Skip adding it now
        }

        if (typeof val !== 'object') {
          pushItem(key, val, false, false);
        } else if (Array.isArray(val) && val.length > 0) {
          const joinedVal = val.join(', ');
          pushItem(key, joinedVal, false, false);
        }
      });

      // Add exterior color at the end of Vehicle Attributes section
      if (exteriorColorValue) {
        items.push({ isHeader: true, value: 'Exterior Color' }); // Header
        items.push({ isHeader: false, isValueOnly: true, value: exteriorColorValue, isFullWidth: true }); // Value row
      }
    }

    // 2. Colors
    if (data.colors && data.colors.length > 0) {
      pushItem('Colors', null, true, false);
      data.colors.forEach((c, index) => {
        const key = c.category ? c.category : `Color ${index + 1}`;
        pushItem(key, c.name, false, false);
      });
    }

    // 3. Equipment
    if (data.equipment) {
      processSection('Equipment', data.equipment);
    }

    // 4. Warranties
    if (data.warranties && data.warranties.length > 0) {
      pushItem('Warranties', null, true, false);
      data.warranties.forEach(w => {
        const key = w.type ? w.type : 'Warranty';
        const value = `${w.miles || '-'} miles / ${w.months || '-'} months`;
        pushItem(key, value, false, false);
      });
    }

    // 5. Deep Data
    if (data.deepdata && Object.keys(data.deepdata).length > 0) {
      pushItem('Deep Data', null, true, false); // Header
      Object.entries(data.deepdata).forEach(([key, val]) => {
        pushItem(key, val, false, false); // Populate deep data values
      });
    }
    this.displayItems = items;
  }
}