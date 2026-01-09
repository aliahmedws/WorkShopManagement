import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { DamageMarkerDetails } from "./damage-marker-details/damage-marker-details";

interface DamageMarkerInterface {
  id: number;
  x: number;
  y: number;
  issueNumber: number;
}

@Component({
  selector: 'app-damage-marker',
  imports: [...SHARED_IMPORTS, DamageMarkerDetails],
  templateUrl: './damage-marker.html',
  styleUrl: './damage-marker.scss'
})
export class DamageMarker implements OnInit {
  @Input() markers: any[] = [];
  @Output() markersChange = new EventEmitter<any[]>();
  // markers: DamageMarkerInterface[] = [];
  nextIssueNumber: number = 1;

  @Input() vin: string | null;
  isModalOpen = false;

  selectedIssue: any = {};
  
  ngOnInit() {
    // Load existing markers from temporary data (simulating fetch from backend)
    this.loadExistingMarkers();
  }

  loadExistingMarkers() {
    // Temporary data - simulating previously saved markers
    // this.markers = [
    //   { id: 1, x: 25.5, y: 30.2, issueNumber: 1 },
    //   { id: 2, x: 65.3, y: 45.8, issueNumber: 2 }
    // ];
    
    // Set next issue number based on existing markers
    if (this.markers.length > 0) {
      this.nextIssueNumber = Math.max(...this.markers.map(m => m.sr)) + 1;
    }
  }

  onImageClick(event: MouseEvent) {
    const imageElement = event.target as HTMLElement;
    const rect = imageElement.getBoundingClientRect();
    
    // Calculate click position relative to image as percentage
    const x = ((event.clientX - rect.left) / rect.width) * 100;
    const y = ((event.clientY - rect.top) / rect.height) * 100;

    // Create new marker
    const newMarker = {
      id: Date.now(), // Using timestamp as unique ID
      x: x,
      y: y,
      sr: this.nextIssueNumber,
      remarks: 'test',
      description: 'test'
    };

    this.markers.push(newMarker);
    this.markersChange.emit(this.markers);
    this.nextIssueNumber++;

    // Here you would typically save to backend
    console.log('New marker added:', newMarker);
    console.log('All markers:', this.markers);
  }

  removeMarker(marker, event: Event) {
    event.stopPropagation(); // Prevent image click event
    this.selectedIssue = this.markers.find(m => m.sr === marker.sr);
    // this.markersChange.emit(this.markers);
    if (this.selectedIssue) this.isModalOpen = true;
    console.log('Marker removed:', marker);
  }

  // Method to simulate saving all markers
  saveMarkers() {
    console.log('Saving markers to backend:', this.markers);
    // Here you would make an HTTP POST request to save markers
  }
}