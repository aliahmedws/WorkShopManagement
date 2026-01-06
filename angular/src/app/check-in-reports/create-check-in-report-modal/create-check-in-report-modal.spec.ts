import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateCheckInReportModal } from './create-check-in-report-modal';

describe('CreateCheckInReportModal', () => {
  let component: CreateCheckInReportModal;
  let fixture: ComponentFixture<CreateCheckInReportModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateCheckInReportModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateCheckInReportModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
