import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { DateTime } from 'luxon';
import { TableComponent } from '../../../../shared/components/table/table.component';
import { MonitoredSystem } from '../../../../shared/models/monitored-system';
import { generateGuid } from '../../../../shared/types/guid/guid';

@Component({
  selector: 'app-monitored-system-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    TableComponent,
  ],
  templateUrl: './monitored-system-list.component.html',
  styleUrl: './monitored-system-list.component.scss'
})
export class MonitoredSystemListComponent {

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
}
