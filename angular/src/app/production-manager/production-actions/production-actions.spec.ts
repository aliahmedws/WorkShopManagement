import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductionActions } from './production-actions';

describe('ProductionActions', () => {
  let component: ProductionActions;
  let fixture: ComponentFixture<ProductionActions>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductionActions]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProductionActions);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
