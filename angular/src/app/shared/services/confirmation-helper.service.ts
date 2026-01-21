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

export const CLEAR_BAY_CONFIRMATION_OPTIONS: Confirmation.Options = {
  yesText: '::Button:Confirm',
  cancelText: '::Button:Cancel',
  icon: 'fas fa-circle-xmark text-warning',
} as const;

export const WARNING_CONFIRMATION_OPTIONS: Confirmation.Options = {
  yesText: '::Button:Yes',
  cancelText: '::Button:No',
  icon: 'fas fa-exclamation-triangle text-warning',
} as const;

export const INFO_CONFIRMATION_OPTIONS: Confirmation.Options = {
  yesText: '::Button:OK',
  hideCancelBtn: true,
  icon: 'fas fa-info-circle text-info',
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

  confirmClearBay(): Observable<Confirmation.Status> {
    return this.confirmation.warn(
      '::ConfirmClearBayMessage',
      '::ConfirmClearBayTitle',
      CLEAR_BAY_CONFIRMATION_OPTIONS
    );
  }

  warn(
    messageKey: string,
    titleKey: string,
    options: Confirmation.Options = WARNING_CONFIRMATION_OPTIONS
  ): Observable<Confirmation.Status> {
    return this.confirmation.warn(messageKey, titleKey, options);
  }

  info(
    messageKey: string,
    titleKey: string,
    options: Confirmation.Options = INFO_CONFIRMATION_OPTIONS
  ): Observable<Confirmation.Status> {
    return this.confirmation.info(messageKey, titleKey, options);
  }

  
}
