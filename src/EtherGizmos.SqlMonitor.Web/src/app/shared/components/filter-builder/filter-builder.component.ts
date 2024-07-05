import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FilterBuilderGroupComponent } from '../filter-builder-group/filter-builder-group.component';

export type FilterOperator = 'and' | 'or';

export interface FilterCondition {
  property: string;
  operator: string;
  value: any;
}

export interface FilterGroup {
  operator: FilterOperator;
  conditions: (FilterCondition | FilterGroup)[];
}

export type FilterType = 'datetime' | 'guid' | 'number' | 'string';

export interface FilterProperty {
  name: string;
  displayName: string;
  type: FilterType;
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

  @Input({ required: true }) properties!: FilterProperty[];
}
