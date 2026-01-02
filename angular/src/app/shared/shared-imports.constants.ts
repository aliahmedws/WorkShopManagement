import { PageModule } from "@abp/ng.components/page";
import { CoreModule } from "@abp/ng.core";
import { ThemeSharedModule } from "@abp/ng.theme.shared";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { NgbDatepickerModule, NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { CopyComponent } from "./components/copy/copy.component";
import { FileUploadComponent } from "./components/file-upload/file-upload.component";
import { TopbarLayoutComponent } from "./components/topbar/topbar-layout.component";

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
    FileUploadComponent
];

export { 
    CopyComponent,
    TopbarLayoutComponent,
    FileUploadComponent
 };