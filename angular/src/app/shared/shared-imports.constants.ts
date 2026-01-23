import { PageModule } from "@abp/ng.components/page";
import { CoreModule } from "@abp/ng.core";
import { EllipsisDirective, ThemeSharedModule } from "@abp/ng.theme.shared";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { NgbDatepickerModule, NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { CopyComponent } from "./components/copy/copy.component";
import { FileUploadComponent } from "./components/file-upload/file-upload.component";
import { TopbarLayoutComponent } from "./components/topbar/topbar-layout.component";
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzImageModule } from 'ng-zorro-antd/image';
import { NzTooltipModule } from 'ng-zorro-antd/tooltip';
import { NzTabsModule } from "ng-zorro-antd/tabs";
import { NzEmptyModule } from "ng-zorro-antd/empty";
import { NzStepsModule } from 'ng-zorro-antd/steps';
import { NzCollapseModule } from 'ng-zorro-antd/collapse';
import { SignedNzImageSrcDirective } from "./services/signed-urls/signed-nz-image-src.directive";
import { EntityChangeHistoryModal } from "../entity-changes/entity-change-history-modal/entity-change-history-modal";

export const SHARED_IMPORTS = [
    CoreModule,
    CommonModule,
    PageModule,
    FormsModule,
    ReactiveFormsModule,
    ThemeSharedModule,
    NgbDatepickerModule,
    NzSelectModule, 
    NgbDropdownModule,
    CopyComponent,
    TopbarLayoutComponent,
    FileUploadComponent,
    NzIconModule,
    NzInputModule,
    NzTagModule,
    NzImageModule,
    NzTooltipModule,
    NzTabsModule,
    NzEmptyModule,
    EllipsisDirective,
    NzStepsModule,
    NzCollapseModule,
    SignedNzImageSrcDirective,
    EntityChangeHistoryModal
];

export { 
    CopyComponent,
    TopbarLayoutComponent,
    FileUploadComponent,
    SignedNzImageSrcDirective, 
    EntityChangeHistoryModal
 };