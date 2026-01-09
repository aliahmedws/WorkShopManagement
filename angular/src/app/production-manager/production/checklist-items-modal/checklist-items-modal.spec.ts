import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChecklistItemsModal } from './checklist-items-modal';

describe('ChecklistItemsModal', () => {
  let component: ChecklistItemsModal;
  let fixture: ComponentFixture<ChecklistItemsModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChecklistItemsModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChecklistItemsModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
