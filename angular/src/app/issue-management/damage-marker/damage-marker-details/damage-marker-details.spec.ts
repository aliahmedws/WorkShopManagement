import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DamageMarkerDetails } from './damage-marker-details';

describe('DamageMarkerDetails', () => {
  let component: DamageMarkerDetails;
  let fixture: ComponentFixture<DamageMarkerDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DamageMarkerDetails]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DamageMarkerDetails);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
