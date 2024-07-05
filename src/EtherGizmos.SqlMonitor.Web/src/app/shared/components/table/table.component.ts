import { CommonModule } from '@angular/common';
import { Component, ContentChild, Input, TemplateRef } from '@angular/core';

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

  getDefaultHeaders(): string[] {
    if (this.data[0]) {
      const keys = Object.keys(this.data[0]);
      return keys;
    } else {
      return [];
    }
  }
}
