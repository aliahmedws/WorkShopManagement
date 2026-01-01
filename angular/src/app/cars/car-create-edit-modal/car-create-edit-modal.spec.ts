import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CarCreateEditModal } from './car-create-edit-modal';

describe('CarCreateEditModal', () => {
  let component: CarCreateEditModal;
  let fixture: ComponentFixture<CarCreateEditModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CarCreateEditModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CarCreateEditModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
