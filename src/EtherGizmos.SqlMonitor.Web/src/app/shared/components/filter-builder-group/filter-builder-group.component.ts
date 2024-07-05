import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FilterCondition, FilterGroup, isFilterGroup } from '../filter-builder/filter-builder.component';

@Component({
  selector: 'filter-builder-group',
  standalone: true,
  imports: [
    CommonModule,
  ],
  templateUrl: './filter-builder-group.component.html',
  styleUrl: './filter-builder-group.component.scss'
})
export class FilterBuilderGroupComponent {

  @Input({ required: true }) filter!: FilterCondition | FilterGroup;
  @Output() filterChange = new EventEmitter<FilterCondition | FilterGroup>;

  get conditions() {
    const group = this.asFilterGroup(this.filter);
    return group.conditions;
  }

  readonly isFilterGroup = isFilterGroup;

  asFilterGroup(condition: FilterCondition | FilterGroup): FilterGroup {
    if (!isFilterGroup(condition))
      throw new Error('Not a filter group');

    return condition;
  }
}
