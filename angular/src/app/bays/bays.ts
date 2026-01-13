import { Component, inject, OnInit } from '@angular/core';
import { BayDto, BayService, GetBayListInput } from '../proxy/bays';
import { ListService } from '@abp/ng.core';
import { SHARED_IMPORTS } from '../shared/shared-imports.constants';
import { Confirmation } from '@abp/ng.theme.shared';
import { ConfirmationHelperService } from '../shared/services/confirmation-helper.service';

@Component({
  selector: 'app-bays',
  imports: [...SHARED_IMPORTS],
  templateUrl: './bays.html',
  styleUrl: './bays.scss',
  providers: [ListService]
})
export class Bays implements OnInit {
  private bayService = inject(BayService);
  private confirmation = inject(ConfirmationHelperService);
  public list = inject(ListService);

  filters: GetBayListInput = { maxResultCount: 1000 };

  bays: BayDto[] = [];

  ngOnInit(): void {
    this.list.maxResultCount = 1000;
    const streamCreator = (query: GetBayListInput) => this.bayService.getList({ ...query, ...this.filters });
    this.list.hookToQuery(streamCreator).subscribe(res => this.bays = res.items || []);
  }

  setIsActive(row: BayDto) {
    if (!row?.id) return;
    this.confirmation
      .confirmAction('::AreYouSureToChangeBayActiveStatus', '::ChangeBayStatus')
      .subscribe(confirm => {
        if (confirm === Confirmation.Status.confirm) {
          this.bayService
            .setIsActive(row.id, !row.isActive)
            .subscribe(() => this.list.get());
        }
      })
  }
}
