import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Recalls } from './recalls';

describe('Recalls', () => {
  let component: Recalls;
  let fixture: ComponentFixture<Recalls>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Recalls]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Recalls);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
