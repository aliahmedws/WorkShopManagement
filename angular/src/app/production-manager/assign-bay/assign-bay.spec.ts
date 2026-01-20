import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignBay } from './assign-bay';

describe('AssignBay', () => {
  let component: AssignBay;
  let fixture: ComponentFixture<AssignBay>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AssignBay]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AssignBay);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
