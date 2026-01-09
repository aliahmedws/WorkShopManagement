import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductionDetailsModal } from './production-details-modal';

describe('ProductionDetailsModal', () => {
  let component: ProductionDetailsModal;
  let fixture: ComponentFixture<ProductionDetailsModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductionDetailsModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProductionDetailsModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
