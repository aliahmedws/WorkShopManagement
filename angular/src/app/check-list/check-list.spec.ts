import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CheckList } from './check-list';

describe('CheckList', () => {
  let component: CheckList;
  let fixture: ComponentFixture<CheckList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CheckList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CheckList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
