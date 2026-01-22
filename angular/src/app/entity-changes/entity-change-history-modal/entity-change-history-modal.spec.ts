import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EntityChangeHistoryModal } from './entity-change-history-modal';

describe('EntityChangeHistoryModal', () => {
  let component: EntityChangeHistoryModal;
  let fixture: ComponentFixture<EntityChangeHistoryModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EntityChangeHistoryModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EntityChangeHistoryModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
