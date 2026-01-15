import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScdWarehouse } from './scd-warehouse';

describe('ScdWarehouse', () => {
  let component: ScdWarehouse;
  let fixture: ComponentFixture<ScdWarehouse>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ScdWarehouse]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ScdWarehouse);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
