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
  // displayItems: DisplayItem[] = []; // Normalized data for UI
  displayItems: DisplayItem[][] = [];
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
    // 1. First, build a flat list using your original logic
    const flatItems: DisplayItem[] = [];

    const formatKey = (str: string) => {
      if (!str) return '';
      return str.replace(/_/g, ' ').replace(/\b\w/g, l => l.toUpperCase());
    };

    const pushItem = (key: string, val: any, isHeader = false, forceFullWidth = false) => {
      if (isHeader) {
        flatItems.push({ isHeader: true, value: key });
      } else {
        const valStr = String(val);
        const isFullWidth = forceFullWidth || valStr.length > 60;
        flatItems.push({
          isHeader: false,
          key: formatKey(key),
          value: valStr,
          isFullWidth: isFullWidth,
          isValueOnly: false
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

    // --- YOUR ORIGINAL PARSING LOGIC STARTS HERE ---

    // 1. Attributes
    let exteriorColorValue: string | null = null;

    if (data.attributes) {
      pushItem('Vehicle Attributes', null, true, false);

      Object.entries(data.attributes).forEach(([key, val]) => {
        if (val === null || val === undefined || val === '') return;

        // Special handling for exterior_color
        if (key.toLowerCase().includes('exterior') && key.toLowerCase().includes('color')) {
          if (Array.isArray(val) && val.length > 0) {
            exteriorColorValue = val.join(', ');
          } else if (typeof val === 'string') {
            exteriorColorValue = val;
          }
          return; 
        }

        if (typeof val !== 'object') {
          pushItem(key, val, false, false);
        } else if (Array.isArray(val) && val.length > 0) {
          const joinedVal = val.join(', ');
          pushItem(key, joinedVal, false, false);
        }
      });

      if (exteriorColorValue) {
        flatItems.push({ isHeader: true, value: 'Exterior Color' });
        flatItems.push({ isHeader: false, isValueOnly: true, value: exteriorColorValue, isFullWidth: true });
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
      pushItem('Deep Data', null, true, false);
      Object.entries(data.deepdata).forEach(([key, val]) => {
        pushItem(key, val, false, false);
      });
    }

    // --- GROUPING LOGIC (The Fix) ---
    
    const groupedRows: DisplayItem[][] = [];
    let buffer: DisplayItem[] = [];

    for (const item of flatItems) {
      // If item is a Header, Full Width, or Value Only, it demands its own row
      if (item.isHeader || item.isFullWidth || item.isValueOnly) {
        // If we have a dangling left-side item in the buffer, force it to be its own row first
        if (buffer.length > 0) {
          groupedRows.push([...buffer]); // Push copy of buffer
          buffer = [];
        }
        // Push the special item as a single-item row
        groupedRows.push([item]);
      } 
      else {
        // Standard item: Add to buffer
        buffer.push(item);
        
        // If buffer has 2 items (Left + Right), flush it as a complete row
        if (buffer.length === 2) {
          groupedRows.push([...buffer]);
          buffer = [];
        }
      }
    }

    // Flush any remaining item
    if (buffer.length > 0) {
      groupedRows.push(buffer);
    }

    this.displayItems = groupedRows;
  }
}