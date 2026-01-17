import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CriticalImagesModal } from './critical-images-modal';

describe('CriticalImagesModal', () => {
  let component: CriticalImagesModal;
  let fixture: ComponentFixture<CriticalImagesModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CriticalImagesModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CriticalImagesModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
