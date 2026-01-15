import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AvvStatusModal } from './avv-status-modal';

describe('AvvStatusModal', () => {
  let component: AvvStatusModal;
  let fixture: ComponentFixture<AvvStatusModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AvvStatusModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AvvStatusModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
