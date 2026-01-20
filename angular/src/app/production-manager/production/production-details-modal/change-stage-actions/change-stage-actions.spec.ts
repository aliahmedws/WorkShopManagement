import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChangeStageActions } from './change-stage-actions';

describe('ChangeStageActions', () => {
  let component: ChangeStageActions;
  let fixture: ComponentFixture<ChangeStageActions>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChangeStageActions]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChangeStageActions);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
