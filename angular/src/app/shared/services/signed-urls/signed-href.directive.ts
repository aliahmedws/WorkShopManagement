// signed-href.directive.ts
import {
  Directive,
  ElementRef,
  Input,
  OnChanges,
  OnDestroy,
  Renderer2,
  SimpleChanges,
  inject,
} from '@angular/core';
import { Subject, catchError, distinctUntilChanged, of, switchMap, takeUntil } from 'rxjs';
import { SignedUrlCacheService } from './signed-url-cache.service';
import { SignedUrlService } from 'src/app/proxy/storage';

@Directive({
  selector: 'a[appSignedHref]',
  standalone: true,
})
export class SignedHrefDirective implements OnChanges, OnDestroy {
  private el = inject(ElementRef<HTMLAnchorElement>);
  private renderer = inject(Renderer2);
  private api = inject(SignedUrlService);
  private cache = inject(SignedUrlCacheService);

  @Input('appSignedHref') source?: string | null;

  // Optional UX
  @Input() signedHrefDisabledClass = 'disabled';
  @Input() signedHrefTarget: '_blank' | '_self' | '_parent' | '_top' = '_blank';

  // Optional: set download attribute (note: cross-origin + signed URLs may still open in new tab depending on headers)
  @Input() signedHrefFileName?: string | null;

  // If you ever want to bypass signing for non-GCS urls, keep this. Default false is safer.
  @Input() signedHrefBypassIfNotGcs = false;

  private readonly lifetimeSeconds = 15 * 60;

  private destroy$ = new Subject<void>();
  private load$ = new Subject<string>();
  private currentKey: string | null = null;
  private subscriptionInitialized = false;

  ngOnChanges(changes: SimpleChanges): void {
    if (!this.subscriptionInitialized) {
      this.subscriptionInitialized = true;
      this.initStream();
    }

    if (!changes['source']) return;

    const key = (this.source ?? '').trim();
    this.currentKey = key || null;

    if (!key) {
      this.setDisabled(true);
      this.setHref('');
      return;
    }

    if (this.signedHrefBypassIfNotGcs && !this.isGoogleCloudStorageRef(key)) {
      this.setDisabled(false);
      this.setHref(key);
      this.applyDownloadAndTarget();
      return;
    }

    // disable while signing
    this.setDisabled(true);
    this.setHref('');

    this.load$.next(key);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private initStream(): void {
    this.load$
      .pipe(
        distinctUntilChanged(),
        switchMap((key) => {
          const cached = this.cache.getValid(key);
          if (cached) {
            this.setDisabled(false);
            this.setHref(cached);
            this.applyDownloadAndTarget();
            return of(null);
          }

          return this.api.getSignedReadUrl(key).pipe(
            catchError(() => {
              // remain disabled if signing fails
              this.setDisabled(true);
              this.setHref('');
              return of(null);
            })
          );
        }),
        takeUntil(this.destroy$)
      )
      .subscribe((signedUrl) => {
        if (!signedUrl || !this.currentKey) return;
        this.cache.set(this.currentKey, signedUrl, this.lifetimeSeconds);
        this.setDisabled(false);
        this.setHref(signedUrl);
        this.applyDownloadAndTarget();
      });
  }

  private setHref(url: string): void {
    this.renderer.setAttribute(this.el.nativeElement, 'href', url);
  }

  private setDisabled(disabled: boolean): void {
    const a = this.el.nativeElement;

    if (disabled) {
      this.renderer.addClass(a, this.signedHrefDisabledClass);
      this.renderer.setAttribute(a, 'aria-disabled', 'true');
      // prevent click navigation while disabled
      this.renderer.setAttribute(a, 'tabindex', '-1');
      this.renderer.setStyle(a, 'pointer-events', 'none');
    } else {
      this.renderer.removeClass(a, this.signedHrefDisabledClass);
      this.renderer.removeAttribute(a, 'aria-disabled');
      this.renderer.removeAttribute(a, 'tabindex');
      this.renderer.removeStyle(a, 'pointer-events');
    }
  }

  private applyDownloadAndTarget(): void {
    const a = this.el.nativeElement;

    if (this.signedHrefTarget) {
      this.renderer.setAttribute(a, 'target', this.signedHrefTarget);
      if (this.signedHrefTarget === '_blank') {
        this.renderer.setAttribute(a, 'rel', 'noopener noreferrer');
      }
    }

    if (this.signedHrefFileName) {
      // Note: download attribute might be ignored depending on cross-origin headers.
      this.renderer.setAttribute(a, 'download', this.signedHrefFileName);
    } else {
      this.renderer.removeAttribute(a, 'download');
    }
  }

  private isGoogleCloudStorageRef(input: string): boolean {
    const s = input.toLowerCase();
    return (
      s.startsWith('gs://') ||
      s.includes('storage.googleapis.com') ||
      s.includes('.storage.googleapis.com')
    );
  }
}