import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Priorities } from './priorities';

describe('Priorities', () => {
  let component: Priorities;
  let fixture: ComponentFixture<Priorities>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Priorities]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Priorities);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
