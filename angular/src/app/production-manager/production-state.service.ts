import { Injectable, inject } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { StageService } from '../proxy/stages/stage.service'; // adjust path

@Injectable({ providedIn: 'root' })
export class ProductionStateService {
  private stageService = inject(StageService);

  private readonly _useClassicView$ = new BehaviorSubject<boolean>(false);
  useClassicView$ = this._useClassicView$.asObservable();

  // optional: prevents repeated initial loads
  private initialized = false;

  init() {
    if (this.initialized) return;
    this.initialized = true;

    this.stageService.getUseProductionClassicView().subscribe(v => {
      this._useClassicView$.next(!!v);
    });
  }

  toggle() {
    const next = !this._useClassicView$.value;

    // update UI immediately
    this._useClassicView$.next(next);

    // persist
    this.stageService.setUseProductionClassicView(next).subscribe({
      error: () => {
        // rollback if save fails
        this._useClassicView$.next(!next);
      }
    });
  }
}