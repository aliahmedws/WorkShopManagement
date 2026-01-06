import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CheckInReport } from './check-in-report';

describe('CheckInReport', () => {
  let component: CheckInReport;
  let fixture: ComponentFixture<CheckInReport>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CheckInReport]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CheckInReport);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
