import { CommonModule } from '@angular/common';
import { Component, ContentChild, Input, TemplateRef } from '@angular/core';
import { SortTableService } from '../../services/sort-table/sort-table.service';
import { generateGuid } from '../../types/guid/guid';

@Component({
  selector: 'app-table',
  standalone: true,
  imports: [
    CommonModule,
  ],
  templateUrl: './table.component.html',
  styleUrl: './table.component.scss'
})
export class TableComponent<TData extends object> {

  @Input({ required: true }) data!: TData[];

  @ContentChild('headers') headers!: TemplateRef<any>;

  @ContentChild('rows') rows!: TemplateRef<any>;

  private readonly $sortTable: SortTableService;

  id = generateGuid();

  constructor(
    $sortTable: SortTableService,
  ) {
    this.$sortTable = $sortTable;
  }

  getDefaultHeaders(): string[] {
    if (this.data[0]) {
      const keys = Object.keys(this.data[0]);
      return keys;
    } else {
      return [];
    }
  }
}
