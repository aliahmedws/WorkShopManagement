import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Bays } from './bays';

describe('Bays', () => {
  let component: Bays;
  let fixture: ComponentFixture<Bays>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Bays]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Bays);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
