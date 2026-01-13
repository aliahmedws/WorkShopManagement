import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Dispatched } from './dispatched';

describe('Dispatched', () => {
  let component: Dispatched;
  let fixture: ComponentFixture<Dispatched>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Dispatched]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Dispatched);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
