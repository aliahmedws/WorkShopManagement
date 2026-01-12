import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PostProduction } from './post-production';

describe('PostProduction', () => {
  let component: PostProduction;
  let fixture: ComponentFixture<PostProduction>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PostProduction]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PostProduction);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
