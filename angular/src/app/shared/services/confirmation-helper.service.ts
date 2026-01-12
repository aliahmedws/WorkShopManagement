// confirmation-helper.service.ts
import { Injectable } from '@angular/core';
import { Confirmation, ConfirmationService } from '@abp/ng.theme.shared';
import { Observable } from 'rxjs';

export const DELETE_CONFIRMATION_OPTIONS: Confirmation.Options = {
    yesText: '::Button:ConfirmDelete',
    icon: 'fas fa-trash text-danger'
} as const;

export const ACTION_CONFIRMATION_OPTIONS: Confirmation.Options = {
  yesText: '::Button:Confirm',
  cancelText: '::Button:Cancel',
  icon: 'fas fa-circle-question text-warning',
} as const;

@Injectable({ providedIn: 'root' })
export class ConfirmationHelperService {
    constructor(private confirmation: ConfirmationService) { }

    /**
     * Shortcut for delete confirmation
     */
    confirmDelete(
        messageKey = '::DeleteConfirmationMessage',
        titleKey = '::DeleteConfirmationTitle'
    ): Observable<Confirmation.Status> {
        return this.confirmation.warn(messageKey, titleKey, DELETE_CONFIRMATION_OPTIONS);
    }

    confirmAction(
    messageKey: string,
    titleKey: string,
    options: Confirmation.Options = ACTION_CONFIRMATION_OPTIONS
  ): Observable<Confirmation.Status> {
    return this.confirmation.warn(messageKey, titleKey, options);
  }
}
