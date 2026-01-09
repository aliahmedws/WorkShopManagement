import { Component, EventEmitter, Input, Output } from '@angular/core';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { DamageMarker } from "../damage-marker/damage-marker";

@Component({
  selector: 'app-issue-modal',
  imports: [...SHARED_IMPORTS, DamageMarker],
  templateUrl: './issue-modal.html',
  styleUrl: './issue-modal.scss'
})
export class IssueModal {
  @Input() visible: boolean;
  @Output() visibleChange = new EventEmitter<boolean>();

  @Input() carId: string | null;

  car: any = {};

  modalOptions = {
    size: 'xl',
    backdrop: 'static', //prevent close by outside click
    keyboard: false, //prevent close by esc key
    animation: true,
  };

  issues = [
    {
      sr: 1,
      x: 25.5,
      y: 30.2,
      description: 'Center console plastic trim scratched on the LHS',
      status: 'Resolved',
      remarks: 'Scratch-dent'
    },
    {
      sr: 2,
      x: 65.3,
      y: 45.8,
      description: "Lock doesn't pop up when locking or unlocking",
      status: 'Resolved',
      remarks: 'Other'
    },
    {
      sr: 3,
      x: 10,
      y: 44,
      description: 'Automatic windows not closing automatically. Window reset performed but issue not resolved',
      status: 'Resolved',
      remarks: 'Other'
    },
    {
      sr: 4,
      x: 45,
      y: 66,
      description: 'Slight steering knock',
      status: 'Resolved',
      remarks: 'Other'
    },
    {
      sr: 5,
      x: 80,
      y: 55,
      description: 'Blend door motor stopper reinforce installed',
      status: 'Resolved',
      remarks: 'Other'
    },
    {
      sr: 6,
      x: 85,
      y: 60,
      description: 'Injection blend door cam lever installed',
      status: 'Resolved',
      remarks: 'Other'
    },
    {
      sr: 7,
      x: 90,
      y: 70,
      description: "DTC's present prior to rework (before clearing)",
      status: 'Resolved',
      remarks: 'DTC'
    },
    {
      sr: 8,
      x: 95,
      y: 80,
      description: "DTC's present prior to rework (after clearing)",
      status: 'Within Limits',
      remarks: 'Normal'
    }
  ];

  appear() {
    this.car.vin = '1FTVW1EL7PWG45779';
  }
}
