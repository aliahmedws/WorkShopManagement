import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductionTopbarActions } from './production-topbar-actions';

describe('ProductionTopbarActions', () => {
  let component: ProductionTopbarActions;
  let fixture: ComponentFixture<ProductionTopbarActions>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductionTopbarActions]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProductionTopbarActions);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
