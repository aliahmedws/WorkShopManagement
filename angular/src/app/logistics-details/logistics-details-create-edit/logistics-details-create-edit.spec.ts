import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LogisticsDetailsCreateEdit } from './logistics-details-create-edit';

describe('LogisticsDetailsCreateEdit', () => {
  let component: LogisticsDetailsCreateEdit;
  let fixture: ComponentFixture<LogisticsDetailsCreateEdit>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LogisticsDetailsCreateEdit]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LogisticsDetailsCreateEdit);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
