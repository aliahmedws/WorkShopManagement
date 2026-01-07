import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Incoming } from './incoming';

describe('Incoming', () => {
  let component: Incoming;
  let fixture: ComponentFixture<Incoming>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Incoming]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Incoming);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
