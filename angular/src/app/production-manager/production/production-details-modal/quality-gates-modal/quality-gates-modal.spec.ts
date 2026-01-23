import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QualityGatesModal } from './quality-gates-modal';

describe('QualityGatesModal', () => {
  let component: QualityGatesModal;
  let fixture: ComponentFixture<QualityGatesModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [QualityGatesModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(QualityGatesModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
