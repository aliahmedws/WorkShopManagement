import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CarExternalResponseModal } from './car-external-response-modal';

describe('CarExternalResponseModal', () => {
  let component: CarExternalResponseModal;
  let fixture: ComponentFixture<CarExternalResponseModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CarExternalResponseModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CarExternalResponseModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
