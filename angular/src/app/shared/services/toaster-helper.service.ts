// toaster-helper.service.ts
import { LocalizationParam } from "@abp/ng.core";
import { ToasterService } from "@abp/ng.theme.shared";
import { Injectable } from "@angular/core";

@Injectable({ providedIn: 'root' })
export class ToasterHelperService {
    constructor(private toaster: ToasterService) { }

    createdOrUpdated(
        id?: string
    ) {
        if (id) this.updated();
        else this.created();
    }

    created(
        messageKey = '',
        titleKey = '::RecordCreatedTitle'
    ) {
        this.success(messageKey, titleKey);
    }

    updated(
        messageKey = '',
        titleKey = '::RecordUpdatedTitle'
    ) {
        this.success(messageKey, titleKey);
    }

    deleted(
        messageKey = '',
        titleKey = '::RecordDeletedTitle'
    ) {
        this.success(messageKey, titleKey);
    }

    assign(
        messageKey = '',
        titleKey = '::AssignSuccessfully'
    ) {
        this.success(messageKey, titleKey);
    }


    error(messageKey: LocalizationParam, titleKey: LocalizationParam) {
        this.toaster.error(messageKey, titleKey);
    }


    success(messageKey: LocalizationParam, titleKey: LocalizationParam) {
        this.toaster.success(messageKey, titleKey);
    }
}
