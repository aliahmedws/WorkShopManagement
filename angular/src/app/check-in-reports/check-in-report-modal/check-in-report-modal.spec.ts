import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CheckInReportModal } from './check-in-report-modal';

describe('CheckInReportModal', () => {
  let component: CheckInReportModal;
  let fixture: ComponentFixture<CheckInReportModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CheckInReportModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CheckInReportModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
