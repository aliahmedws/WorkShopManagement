import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateCar } from './create-car';

describe('CreateCar', () => {
  let component: CreateCar;
  let fixture: ComponentFixture<CreateCar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateCar]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateCar);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
