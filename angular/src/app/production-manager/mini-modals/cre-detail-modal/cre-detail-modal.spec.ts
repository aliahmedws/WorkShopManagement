import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreDetailModal } from './cre-detail-modal';

describe('CreDetailModal', () => {
  let component: CreDetailModal;
  let fixture: ComponentFixture<CreDetailModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreDetailModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreDetailModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
