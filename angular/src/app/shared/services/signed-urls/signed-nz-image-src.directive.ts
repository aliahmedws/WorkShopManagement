// signed-nz-image-src.directive.ts
import {
    Directive,
    ElementRef,
    Input,
    OnChanges,
    OnDestroy,
    Optional,
    Renderer2,
    Self,
    SimpleChanges,
    inject,
} from '@angular/core';
import { Subject, catchError, distinctUntilChanged, of, switchMap, takeUntil } from 'rxjs';
import { SignedUrlCacheService } from './signed-url-cache.service';

// Adjust if your import path differs
import { NzImageDirective } from 'ng-zorro-antd/image';
import { SignedUrlService } from 'src/app/proxy/storage';

@Directive({
    selector: 'img[nz-image][appSignedNzSrc]',
    standalone: true,
})
export class SignedNzImageSrcDirective implements OnChanges, OnDestroy {
    private el = inject(ElementRef<HTMLImageElement>);
    private renderer = inject(Renderer2);
    private api = inject(SignedUrlService);
    private cache = inject(SignedUrlCacheService);

    // Optional: if present, keeps nz-image preview behavior consistent
    constructor(@Optional() @Self() private nzImage?: NzImageDirective) { }

    @Input('appSignedNzSrc') source?: string | null;
    @Input() signedNzPlaceholder?: string | null;
    @Input() signedNzBypassIfPublic = true;

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
            this.applyUrl(this.signedNzPlaceholder ?? '');
            return;
        }

        if (this.signedNzBypassIfPublic && this.looksFetchable(key)) {
            this.applyUrl(key);
            return;
        }

        // Show placeholder immediately
        if (this.signedNzPlaceholder) this.applyUrl(this.signedNzPlaceholder);

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
                        this.applyUrl(cached);
                        return of(null);
                    }

                    return this.api.getSignedReadUrl(key).pipe(
                        catchError(() => {
                            this.applyUrl(this.signedNzPlaceholder ?? '');
                            return of(null);
                        })
                    );
                }),
                takeUntil(this.destroy$)
            )
            .subscribe((signedUrl) => {
                if (!signedUrl || !this.currentKey) return;
                this.cache.set(this.currentKey, signedUrl, this.lifetimeSeconds);
                this.applyUrl(signedUrl);
            });
    }

    private applyUrl(url: string): void {
        // 1) This is what actually makes the image render
        this.renderer.setAttribute(this.el.nativeElement, 'src', url);

        // 2) Keep nz-image in sync for preview/zoom
        if (this.nzImage) {
            (this.nzImage as any).nzSrc = url;

            // Some versions rely on ngOnChanges; call it if present
            (this.nzImage as any).ngOnChanges?.({
                nzSrc: {
                    currentValue: url,
                    previousValue: null,
                    firstChange: false,
                    isFirstChange: () => false,
                },
            });
        }
    }

    private looksFetchable(input: string): boolean {
        const s = input.toLowerCase();
        return s.startsWith('data:') || s.startsWith('blob:') || s.startsWith('assets/') || s.startsWith('/assets/');
    }
}