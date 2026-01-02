import { PageModule } from "@abp/ng.components/page";
import { CoreModule } from "@abp/ng.core";
import { ThemeSharedModule } from "@abp/ng.theme.shared";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { NgbDatepickerModule, NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { NzSelectModule } from 'ng-zorro-antd/select';

export const SHARED_IMPORTS = [
    CoreModule,
    CommonModule,
    PageModule,
    FormsModule,
    ThemeSharedModule,
    NgbDatepickerModule,
    NzSelectModule, 
    NgbDropdownModule
];