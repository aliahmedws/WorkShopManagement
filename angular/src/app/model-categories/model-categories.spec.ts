import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModelCategories } from './model-categories';

describe('ModelCategories', () => {
  let component: ModelCategories;
  let fixture: ComponentFixture<ModelCategories>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ModelCategories]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ModelCategories);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
