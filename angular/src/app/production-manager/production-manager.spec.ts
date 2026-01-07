import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductionManager } from './production-manager';

describe('ProductionManager', () => {
  let component: ProductionManager;
  let fixture: ComponentFixture<ProductionManager>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductionManager]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProductionManager);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
