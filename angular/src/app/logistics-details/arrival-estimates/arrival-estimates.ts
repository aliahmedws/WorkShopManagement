import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { ActivatedRoute} from '@angular/router';
import { ArrivalEstimateService, ArrivalEstimateDto } from 'src/app/proxy/logistics-details/arrival-estimates';
import { ConfirmationHelperService } from 'src/app/shared/services/confirmation-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { ArrivalEstimatesCreateEditModal } from './arrival-estimates-create-edit-modal/arrival-estimates-create-edit-modal';

@Component({
  selector: 'app-arrival-estimates',
  standalone: true,
  imports: [...SHARED_IMPORTS, 
    ArrivalEstimatesCreateEditModal
  ],
  templateUrl: './arrival-estimates.html',
  styleUrl: './arrival-estimates.scss',
  providers: [ListService]
})
export class ArrivalEstimates {
  public readonly list = inject(ListService);
  private readonly service = inject(ArrivalEstimateService);
  private readonly route = inject(ActivatedRoute);
  private readonly confirmation = inject(ConfirmationHelperService);

  data: PagedResultDto<ArrivalEstimateDto> = { items: [], totalCount: 0 };
  
  carId: string | null = null; // To go back

  @Input() logisticsDetailId: string | null = null;

  @Input() visible = false;
  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() submit = new EventEmitter<void>();
  
  // Modal State
  isCreateEditModalVisible = false;
  selectedId?: string;

  modalOptions = {
    size: 'xl',
    backdrop: 'static',
    keyboard: false
  };

  open(): void {

    if (!this.logisticsDetailId) {
      this.close();
      return;
    }

    const streamCreator = (query: any) => this.service.getList(this.logisticsDetailId!, query);
    this.list.hookToQuery(streamCreator).subscribe(res => this.data = res);
  }

  create() {
    this.selectedId = undefined;
    this.isCreateEditModalVisible = true;
  }

  edit(id: string) {
    this.selectedId = id;
    this.isCreateEditModalVisible = true;
  }

  delete(id: string) {
    this.confirmation.confirmDelete().subscribe(status => {
      if (status === 'confirm') {
        this.service.delete(id).subscribe(() => this.list.get());
      }
    });
  }

  close() {
    this.visible = false;
    this.visibleChange.emit(this.visible);
  }
}