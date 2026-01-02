import { ListService, LocalizationPipe } from '@abp/ng.core';
import { booleanAttribute, Component, EventEmitter, Input, Output } from '@angular/core';
import { localizeKey } from '../../utils/localization.utils';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-topbar-layout',
  imports: [LocalizationPipe, FormsModule],
  templateUrl: './topbar-layout.component.html',
  styleUrl: './topbar-layout.component.scss'
})
export class TopbarLayoutComponent {
  @Input({ transform: localizeKey }) titleKey!: string;

  @Input({ transform: booleanAttribute }) showSearch = true;
  @Input({ transform: booleanAttribute }) showFiltersToggle = false;
  @Input({ transform: booleanAttribute }) showApplyFilters = true;
  @Input({ transform: booleanAttribute }) showResetFilters = true;

  @Input({ transform: localizeKey }) filtersTitleKey = '::Filters';

  @Input() list: ListService;

  @Input() filters: any = {};
  @Output() filtersChange = new EventEmitter<any>();
  
  @Input({ transform: booleanAttribute }) filtersVisible: boolean = false;
  @Output() filtersVisibleChange = new EventEmitter<boolean>();

  @Output() onReset = new EventEmitter<any>();
  @Output() onFilter = new EventEmitter<any>();

  @Input({ transform: localizeKey }) searchPlaceholder = '::Search';

  toggleFilters() {
    this.filtersVisible = !this.filtersVisible;
    this.filtersVisibleChange.emit(this.filtersVisible);
  }

  reset() {
    this.filters = { asNoTracking: true, includeDetails: true };
    this.filtersChange.emit(this.filters);
    this.onReset.emit();
    this.list?.get();
  }

  filter() {
    if (!this.filters) return;
    this.filtersChange.emit(this.filters);
    this.onFilter.emit(this.filters);
    this.list?.get();
  }
}
