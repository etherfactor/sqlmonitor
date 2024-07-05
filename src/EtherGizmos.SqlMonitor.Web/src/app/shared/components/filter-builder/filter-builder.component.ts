import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FilterBuilderGroupComponent } from '../filter-builder-group/filter-builder-group.component';

export type FilterOperator = 'AND' | 'OR';

export interface FilterCondition {
  property: string;
  operator: string;
  value: any;
}

export interface FilterGroup {
  operator: FilterOperator;
  conditions: (FilterCondition | FilterGroup)[];
}

export function isFilterGroup(condition: FilterCondition | FilterGroup): condition is FilterGroup {
  return (condition as FilterGroup).conditions !== undefined;
}

@Component({
  selector: 'filter-builder',
  standalone: true,
  imports: [
    FilterBuilderGroupComponent,
  ],
  templateUrl: './filter-builder.component.html',
  styleUrl: './filter-builder.component.scss'
})
export class FilterBuilderComponent {

  @Input({ required: true }) filter!: FilterGroup;
  @Output() filterChange = new EventEmitter<FilterGroup>();
}
