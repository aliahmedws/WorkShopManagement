import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormsModule } from '@angular/forms';
import { ListService, LocalizationPipe, PagedResultDto } from '@abp/ng.core';
import { PriorityService, PriorityDto, GetPriorityListDto } from '../proxy/priorities';
import { Confirmation, ConfirmationService, ToasterService } from '@abp/ng.theme.shared';
import { CommonModule } from '@angular/common';
import { PageModule } from '@abp/ng.components/page';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { ReactiveFormsModule } from '@angular/forms';
import { PermissionDirective } from '@abp/ng.core';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';


@Component({
  standalone: true,
  selector: 'app-priorities',
  templateUrl: './priorities.html',
  styleUrls: ['./priorities.scss'],
  imports: [
    CommonModule,
    PageModule,
    ThemeSharedModule,
    ReactiveFormsModule,
     FormsModule,
    LocalizationPipe,
    NgbDropdownModule,
    PermissionDirective
  ],
  providers: [ListService]
})
export class Priorities implements OnInit {
  priorities = { items: [], totalCount: 0 } as PagedResultDto<PriorityDto>;
  isModalOpen = false;
  showFilter = false;
  form: FormGroup;
  selectedPriority = {} as PriorityDto;
  filters = {} as GetPriorityListDto;

  constructor(
    public readonly list: ListService,
    private priorityService: PriorityService,
    private fb: FormBuilder,
    private confirmation: ConfirmationService,
    private toaster: ToasterService
  ) {}

  ngOnInit(): void {
    const priorityStreamCreator = (query) => {
      return this.priorityService.getList({ ...query, ...this.filters });
    };

    this.list.hookToQuery(priorityStreamCreator).subscribe((response) => {
      this.priorities = response;
    });
  }

  buildForm(): void {
    this.form = this.fb.group({
      number: [this.selectedPriority.number || '', Validators.required],
      description: [this.selectedPriority.description || '']
    });
  }

  createPriority(): void {
    this.selectedPriority = {} as PriorityDto;
    this.buildForm();
    this.isModalOpen = true;
  }

  editPriority(id: string): void {
    this.priorityService.get(id).subscribe((priority) => {
      this.selectedPriority = priority;
      this.buildForm();
      this.isModalOpen = true;
    });
  }

  save(): void {
    if (this.form.invalid) return;

    if (this.selectedPriority.id && !this.form.dirty) {
      this.toaster.info('::NoChangesDetected');
      return;
    }

    if (this.selectedPriority.id) {
        this.priorityService.update(this.selectedPriority.id, this.form.value).subscribe(() => {
        this.isModalOpen = false;
        this.form.reset();
        this.list.get();
        this.toaster.success('::PriorityUpdatedSuccessfully');
      });
    } else {
        this.priorityService.create(this.form.value).subscribe(() => {
        this.isModalOpen = false;
        this.form.reset();
        this.list.get();
        this.toaster.success('::PriorityCreatedSuccessfully');
      });
    }
  }

  delete(id: string): void {
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.priorityService.delete(id).subscribe(() => {
          this.list.get();
          this.toaster.success('::PriorityDeletedSuccessfully');
        });
      }
    });
  }

  clearFilters(): void {
    this.filters = {} as GetPriorityListDto;
    this.list.get();
     if (this.form) {  
       this.form.reset();
    }
}
}
