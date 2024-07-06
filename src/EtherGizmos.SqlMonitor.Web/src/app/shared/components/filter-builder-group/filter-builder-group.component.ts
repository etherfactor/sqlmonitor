import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { FilterCondition, FilterGroup, FilterProperty, FilterPropertyOperator, defaultOperators, displayText, isFilterGroup } from '../filter-builder/filter-builder.component';
import { InputLuxonDatetimeComponent } from '../input-luxon-datetime/input-luxon-datetime.component';

@Component({
  selector: 'filter-builder-group',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InputLuxonDatetimeComponent,
    NgSelectModule,
  ],
  templateUrl: './filter-builder-group.component.html',
  styleUrl: './filter-builder-group.component.scss'
})
export class FilterBuilderGroupComponent {

  @Input({ required: true }) filter!: FilterCondition | FilterGroup;
  @Output() filterChange = new EventEmitter<FilterCondition | FilterGroup>;

  @Input({ required: true }) properties!: FilterProperty[];

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

  asFilterCondition(condition: FilterCondition | FilterGroup): FilterCondition {
    if (isFilterGroup(condition))
      throw new Error('Not a filter condition');

    return condition;
  }

  get selectedProperty() {
    const condition = this.asFilterCondition(this.filter);
    return this.properties.find(e => e.name === condition.property)!;
  }

  getOperators(): FilterPropertyOperator[] {
    const property = this.selectedProperty;
    return property.operators ?? defaultOperators[property.type];
  }

  getOperatorDisplayName(operator: FilterPropertyOperator) {
    const property = this.selectedProperty;
    return displayText[property.type][operator];
  }
}
