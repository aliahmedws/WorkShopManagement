import { ListResultDto, PermissionService } from '@abp/ng.core';
import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { EntityChangeService, EntityChangeWithUsernameDto, EntityPropertyChangeDto } from 'src/app/proxy/entity-changes';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { resolveEntityTypeFullName } from '../entity-type.functions';
import { NgbAccordionModule } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-entity-change-history-modal',
  imports: [...SHARED_IMPORTS, NgbAccordionModule],
  templateUrl: './entity-change-history-modal.html',
  styleUrl: './entity-change-history-modal.scss'
})
export class EntityChangeHistoryModal implements OnInit {
  private readonly service = inject(EntityChangeService);
  private readonly permission = inject(PermissionService);

  @Input() visible: boolean;
  @Output() visibleChange = new EventEmitter<boolean>();

  @Input() entityId: string;
  @Input() entityName: 'Car' | 'Issue' | 'CarBay' | 'CarBayItem';

  @Input() buttonType: 'icon' | 'button' | 'dropdown' = 'icon';

  entityTypeFullName = '';

  changes: ListResultDto<EntityChangeWithUsernameDto> = {};

  modalOptions = {
    size: 'xl',
    backdrop: 'static',
    keyboard: false,
    animation: true,
  };

  get isValidInput(): boolean {
    return !!this.entityId && !!this.entityName;
  }

  loading = false;

  isAuthorized = false;

  ngOnInit(): void {
    this.isAuthorized = this.permission.getGrantedPolicy('WorkShopManagement.AuditLogs') || false;
  }

  showModal() {
    if (!this.isAuthorized) return;
    this.open();
    this.entityTypeFullName = resolveEntityTypeFullName(this.entityName);
    if (!this.entityId || !this.entityTypeFullName) return;
    this.loading = true;

    this.service
      .getChangeHistory(this.entityId, this.entityTypeFullName)
      .subscribe((res) => {
        this.changes = res || {};
        this.loading = false;
      });
  }

  open() {
    this.visible = true;
    this.visibleChange.emit(true);
  }

  close() {
    this.visible = false;
    this.visibleChange.emit(false);
  }

  disappear() {
    this.changes = {};
  }

  openItem(item: any) {
    const itemValue = !!item.isOpen;
    this.changes.items.map((i: any) => i.isOpen = false);
    item.isOpen = !itemValue;
  }

  getProperties(item: EntityChangeWithUsernameDto) {
    return ((item?.entityChange?.propertyChanges || []).filter(p => this.hasValue(p)) || [])
  }

  hasValue(prop: EntityPropertyChangeDto | null) {
    return !!prop && ((prop.originalValue?.trim() && prop.originalValue !== 'null') || (prop.newValue?.trim() && prop.newValue !== 'null'))
  }

  getValue(value: string | null) {
    return value?.trim() && value !== 'null' ? value : '-'
  }
}
