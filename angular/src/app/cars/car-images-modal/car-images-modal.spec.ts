import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CarImagesModal } from './car-images-modal';

describe('CarImagesModal', () => {
  let component: CarImagesModal;
  let fixture: ComponentFixture<CarImagesModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CarImagesModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CarImagesModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
