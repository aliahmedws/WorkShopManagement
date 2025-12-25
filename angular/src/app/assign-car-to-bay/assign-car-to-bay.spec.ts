import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignCarToBay } from './assign-car-to-bay';

describe('AssignCarToBay', () => {
  let component: AssignCarToBay;
  let fixture: ComponentFixture<AssignCarToBay>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AssignCarToBay]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AssignCarToBay);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
