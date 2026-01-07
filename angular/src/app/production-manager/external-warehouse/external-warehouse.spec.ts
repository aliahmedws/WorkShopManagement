import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExternalWarehouse } from './external-warehouse';

describe('ExternalWarehouse', () => {
  let component: ExternalWarehouse;
  let fixture: ComponentFixture<ExternalWarehouse>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ExternalWarehouse]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ExternalWarehouse);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
