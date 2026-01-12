import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ArrivalEstimates } from './arrival-estimates';

describe('ArrivalEstimates', () => {
  let component: ArrivalEstimates;
  let fixture: ComponentFixture<ArrivalEstimates>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ArrivalEstimates]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ArrivalEstimates);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
