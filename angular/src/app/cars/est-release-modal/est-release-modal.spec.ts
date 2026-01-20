import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EstReleaseModal } from './est-release-modal';

describe('EstReleaseModal', () => {
  let component: EstReleaseModal;
  let fixture: ComponentFixture<EstReleaseModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EstReleaseModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EstReleaseModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
