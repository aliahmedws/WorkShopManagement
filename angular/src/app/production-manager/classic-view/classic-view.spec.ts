import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClassicView } from './classic-view';

describe('ClassicView', () => {
  let component: ClassicView;
  let fixture: ComponentFixture<ClassicView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClassicView]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClassicView);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
