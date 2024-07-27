import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DateTime } from 'luxon';
import { FilterBuilderModalComponent, FilterGroup, FilterProperty, filterGroupForm } from '../../../../shared/components/filter-builder-modal/filter-builder-modal.component';
import { TableSortHeaderComponent } from '../../../../shared/components/table-sort-header/table-sort-header.component';
import { TableComponent } from '../../../../shared/components/table/table.component';
import { MonitoredSystem } from '../../../../shared/models/monitored-system';
import { Guid, generateGuid } from '../../../../shared/types/guid/guid';
import { DefaultControlTypes, TypedFormGroup } from '../../../../shared/utilities/form/form.util';

@Component({
  selector: 'app-monitored-system-list',
  standalone: true,
  imports: [
    CommonModule,
    FilterBuilderModalComponent,
    RouterModule,
    TableComponent,
    TableSortHeaderComponent,
  ],
  templateUrl: './monitored-system-list.component.html',
  styleUrl: './monitored-system-list.component.scss'
})
export class MonitoredSystemListComponent {

  private readonly $form: FormBuilder;
  private readonly $modal: NgbModal;

  filterForm: TypedFormGroup<FilterGroup, DefaultControlTypes>;

  filter: FilterGroup;
  properties: FilterProperty[];

  records: MonitoredSystem[] = [
    {
      id: 'fca5315f-6e2f-4a78-baac-bdb061e6d8fc' as Guid,
      createdAt: DateTime.now(),
      createdByUserId: generateGuid(),
      modifiedAt: undefined,
      modifiedByUserId: undefined,
      name: 'Example System',
      description: 'Desc',
      isActive: true,
    }
  ];

  constructor(
    $form: FormBuilder,
    $modal: NgbModal,
  ) {
    this.$form = $form;
    this.$modal = $modal;

    this.filter = {
      operator: 'and',
      conditions: [
        { property: undefined!, operator: undefined!, value: undefined! },
      ]
    };

    this.properties = [
      { name: 'id', displayName: 'Id', type: 'guid' },
      { name: 'name', displayName: 'Name', type: 'string' },
      { name: 'description', displayName: 'Description', type: 'string' },
      { name: 'isActive', displayName: 'Is active', type: 'boolean' },
      { name: 'createdAt', displayName: 'Created at', type: 'datetime' },
      { name: 'createdByUserId', displayName: 'Created by user id', type: 'guid' },
      { name: 'modifiedAt', displayName: 'Modified at', type: 'datetime' },
      { name: 'modifiedByUserId', displayName: 'Modified by user id', type: 'guid' },
    ];

    this.filterForm = filterGroupForm(this.$form, this.filter);
  }

  openFilters() {
    const modalInstance = this.$modal.open(FilterBuilderModalComponent, { size: 'lg', centered: true, backdrop: 'static', keyboard: false });

    const component = modalInstance.componentInstance as FilterBuilderModalComponent;
    component.setFilter(this.filter, this.properties);

    modalInstance.result.then(
      (result: FilterGroup) => {
        console.log(result);
        this.filter = result;
      },
      dismissed => { }
    );
  }
}
