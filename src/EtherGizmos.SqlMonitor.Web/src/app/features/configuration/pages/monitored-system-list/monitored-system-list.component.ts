import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { DateTime } from 'luxon';
import { FilterBuilderComponent, FilterGroup, filterGroupForm } from '../../../../shared/components/filter-builder/filter-builder.component';
import { TableComponent } from '../../../../shared/components/table/table.component';
import { MonitoredSystem } from '../../../../shared/models/monitored-system';
import { generateGuid } from '../../../../shared/types/guid/guid';
import { DefaultControlTypes, TypedFormGroup } from '../../../../shared/utilities/form/form.util';

@Component({
  selector: 'app-monitored-system-list',
  standalone: true,
  imports: [
    CommonModule,
    FilterBuilderComponent,
    RouterModule,
    TableComponent,
  ],
  templateUrl: './monitored-system-list.component.html',
  styleUrl: './monitored-system-list.component.scss'
})
export class MonitoredSystemListComponent {

  private readonly $form: FormBuilder;

  filterForm: TypedFormGroup<FilterGroup, DefaultControlTypes>;

  records: MonitoredSystem[] = [
    {
      id: generateGuid(),
      createdAt: DateTime.now(),
      createdByUserId: generateGuid(),
      modifiedAt: undefined,
      modifiedByUserId: undefined,
      name: 'Test',
      description: 'Desc',
      isActive: true,
    }
  ];

  constructor(
    $form: FormBuilder,
  ) {
    this.$form = $form;

    const newFilter: FilterGroup = {
      operator: 'and',
      conditions: [
        { property: 'name', operator: 'starts_with', value: 'A' },
        {
          operator: 'or', conditions: [
            { property: 'name', operator: 'equals', value: 'A' },
            { property: 'name', operator: 'not_equals', value: 'B' }
          ]
        }
      ]
    };

    this.filterForm = filterGroupForm(this.$form, newFilter);
  }
}
