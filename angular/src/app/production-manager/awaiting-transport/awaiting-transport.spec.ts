import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AwaitingTransport } from './awaiting-transport';

describe('AwaitingTransport', () => {
  let component: AwaitingTransport;
  let fixture: ComponentFixture<AwaitingTransport>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AwaitingTransport]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AwaitingTransport);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
