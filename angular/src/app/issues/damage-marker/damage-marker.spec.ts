import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DamageMarker } from './damage-marker';

describe('DamageMarker', () => {
  let component: DamageMarker;
  let fixture: ComponentFixture<DamageMarker>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DamageMarker]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DamageMarker);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
