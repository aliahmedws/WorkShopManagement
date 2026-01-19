import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ArrivalEstimateService, ArrivalEstimateDto } from 'src/app/proxy/logistics-details/arrival-estimates';
import { ConfirmationHelperService } from 'src/app/shared/services/confirmation-helper.service';
import { SHARED_IMPORTS } from 'src/app/shared/shared-imports.constants';
import { ArrivalEstimatesCreateEditModal } from './arrival-estimates-create-edit-modal/arrival-estimates-create-edit-modal';

@Component({
  selector: 'app-arrival-estimates',
  standalone: true,
  imports: [...SHARED_IMPORTS, ArrivalEstimatesCreateEditModal],
  templateUrl: './arrival-estimates.html',
  styleUrl: './arrival-estimates.scss',
  providers: [ListService]
})
export class ArrivalEstimates implements OnInit {
  public readonly list = inject(ListService);
  private readonly service = inject(ArrivalEstimateService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly confirmation = inject(ConfirmationHelperService);

  data: PagedResultDto<ArrivalEstimateDto> = { items: [], totalCount: 0 };
  
  logisticsDetailId: string | null = null;
  carId: string | null = null; // To go back
  
  // Modal State
  isModalVisible = false;
  selectedId?: string;

  ngOnInit(): void {
    this.logisticsDetailId = this.route.snapshot.queryParamMap.get('logisticsDetailId');
    this.carId = this.route.snapshot.queryParamMap.get('carId');

    if (!this.logisticsDetailId) {
      this.goBack();
      return;
    }

    const streamCreator = (query: any) => this.service.getList(this.logisticsDetailId!, query);
    this.list.hookToQuery(streamCreator).subscribe(res => this.data = res);
  }

  create() {
    this.selectedId = undefined;
    this.isModalVisible = true;
  }

  edit(id: string) {
    this.selectedId = id;
    this.isModalVisible = true;
  }

  delete(id: string) {
    this.confirmation.confirmDelete().subscribe(status => {
      if (status === 'confirm') {
        this.service.delete(id).subscribe(() => this.list.get());
      }
    });
  }

  goBack() {
    if (this.carId) {
      this.router.navigate(['/logistics-details'], { queryParamsHandling: 'merge' });
    } else {
      this.router.navigate(['/cars']);
    }
  }
}