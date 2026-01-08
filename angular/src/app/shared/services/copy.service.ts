// copy.service.ts
import { ToasterService } from '@abp/ng.theme.shared';
import { inject, Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root',
})
export class CopyService {

    private toaster = inject(ToasterService);
    /**
     * Copies the given text to the clipboard.
     * Returns true on success, false on failure.
     */
    async copy(text: string, showToaster: boolean = true): Promise<boolean> {
        if (!text && text !== '') {
            return false;
        }

        try {
            // Modern async Clipboard API
            if (this.canUseClipboardApi()) {
                await navigator.clipboard.writeText(text);
                this.showToaster(text, showToaster);
                return true;
            }

            // Fallback for older browsers
            return this.copyWithFallback(text, showToaster);
        } catch {
            return this.copyWithFallback(text, showToaster);
        }
    }

    private showToaster(text: string, showToaster: boolean) {
        if (showToaster) {
            this.toaster.success('::CopyService:Copied', '', { messageLocalizationParams: [text] })
        }
    }

    private canUseClipboardApi(): boolean {
        return !!navigator && !!navigator.clipboard && !!navigator.clipboard.writeText;
    }

    private copyWithFallback(text: string, showToaster: boolean): boolean {
        try {
            const textarea = document.createElement('textarea');
            textarea.value = text;

            // Avoid scrolling to bottom
            textarea.style.position = 'fixed';
            textarea.style.left = '-9999px';
            textarea.style.top = '0';

            document.body.appendChild(textarea);
            textarea.focus();
            textarea.select();

            const successful = document.execCommand('copy');
            document.body.removeChild(textarea);

            this.showToaster(text, showToaster);
            return successful;
        } catch {
            return false;
        }
    }
}
