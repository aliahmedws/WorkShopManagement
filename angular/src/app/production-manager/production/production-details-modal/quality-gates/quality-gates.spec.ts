import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QualityGates } from './quality-gates';

describe('QualityGates', () => {
  let component: QualityGates;
  let fixture: ComponentFixture<QualityGates>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [QualityGates]
    })
    .compileComponents();

    fixture = TestBed.createComponent(QualityGates);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
