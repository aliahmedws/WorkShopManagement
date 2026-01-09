import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IssueStatusBadge } from './issue-status-badge';

describe('IssueStatusBadge', () => {
  let component: IssueStatusBadge;
  let fixture: ComponentFixture<IssueStatusBadge>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [IssueStatusBadge]
    })
    .compileComponents();

    fixture = TestBed.createComponent(IssueStatusBadge);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
