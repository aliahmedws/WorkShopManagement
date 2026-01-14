import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CarNotesModal } from './car-notes-modal';

describe('CarNotesModal', () => {
  let component: CarNotesModal;
  let fixture: ComponentFixture<CarNotesModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CarNotesModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CarNotesModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
