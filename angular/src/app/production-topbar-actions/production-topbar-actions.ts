import { LocalizationPipe, PermissionDirective } from '@abp/ng.core';
import { Component, inject, OnInit } from '@angular/core';
import { StageService } from '../proxy/stages';
import { ToasterService } from '@abp/ng.theme.shared';
import { ProductionStateService } from '../production-manager/production-state.service';
import { AsyncPipe, CommonModule } from '@angular/common';

@Component({
  selector: 'app-production-topbar-actions',
  imports: [LocalizationPipe, CommonModule, PermissionDirective],
  templateUrl: './production-topbar-actions.html',
  styleUrl: './production-topbar-actions.scss'
})
export class ProductionTopbarActions implements OnInit {
  private stageService = inject(StageService);
  private toaster = inject(ToasterService);
  private state = inject(ProductionStateService);

  useClassicView = false;

  ngOnInit(): void {
    this.state.useClassicView$
      .subscribe(useClassicView =>
        this.useClassicView = useClassicView || false
      );
  }

  toggleView() {
    this.state.toggle();
  }

  export() {
    this.stageService
      .getListAsExcel()
      .subscribe({
        next: (resp) => {
          const blob = new Blob([resp], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
          const url = window.URL.createObjectURL(blob);
          const link = document.createElement('a');
          link.href = url;
          link.download = `Production_Manager_${new Date().toISOString()}.xlsx`;
          link.click();

          document.body.removeChild(link);
          window.URL.revokeObjectURL(url);
        },
        error: (err) => {
          this.toaster.error('Something went wrong while downloading file.')
        }
      })
  }
}
