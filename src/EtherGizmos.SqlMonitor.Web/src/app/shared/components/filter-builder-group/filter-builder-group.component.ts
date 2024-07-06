import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { DefaultControlTypes, TypedFormGroup } from '../../utilities/form/form.util';
import { FilterCondition, FilterGroup, FilterProperty, FilterPropertyOperator, defaultOperators, displayText, filterConditionForm, filterGroupForm, isFilterGroupForm } from '../filter-builder/filter-builder.component';
import { InputLuxonDatetimeComponent } from '../input-luxon-datetime/input-luxon-datetime.component';

@Component({
  selector: 'filter-builder-group',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InputLuxonDatetimeComponent,
    NgSelectModule,
    ReactiveFormsModule,
  ],
  templateUrl: './filter-builder-group.component.html',
  styleUrl: './filter-builder-group.component.scss'
})
export class FilterBuilderGroupComponent {

  private readonly $form: FormBuilder;

  @Input() root: boolean = false;

  @Input({ required: true }) filter!: TypedFormGroup<FilterCondition, DefaultControlTypes> | TypedFormGroup<FilterGroup, DefaultControlTypes>;

  @Input({ required: true }) properties!: FilterProperty[];

  constructor(
    $form: FormBuilder,
  ) {
    this.$form = $form;
  }

  get conditions() {
    const group = this.asFilterGroup(this.filter);
    return group.controls.conditions;
  }

  readonly isFilterGroupForm = isFilterGroupForm;

  asFilterGroup(condition: TypedFormGroup<FilterCondition, DefaultControlTypes> | TypedFormGroup<FilterGroup, DefaultControlTypes>): TypedFormGroup<FilterGroup, DefaultControlTypes> {
    if (!isFilterGroupForm(condition))
      throw new Error('Not a filter group');

    return condition;
  }

  asFilterCondition(condition: TypedFormGroup<FilterCondition, DefaultControlTypes> | TypedFormGroup<FilterGroup, DefaultControlTypes>): TypedFormGroup<FilterCondition, DefaultControlTypes> {
    if (isFilterGroupForm(condition))
      throw new Error('Not a filter condition');

    return condition;
  }

  get selectedProperty() {
    const condition = this.asFilterCondition(this.filter);
    return this.properties.find(e => e.name === condition.value.property);
  }

  getOperators(): FilterPropertyOperator[] {
    const property = this.selectedProperty;
    return property?.operators ?? defaultOperators[property?.type ?? 'string'];
  }

  getOperatorDisplayName(operator: FilterPropertyOperator) {
    const property = this.selectedProperty;
    return displayText[property?.type ?? 'string'][operator];
  }

  addGroup() {
    const group = this.asFilterGroup(this.filter);

    const newCondition: FilterCondition = {
      property: null!,
      operator: 'equals',
      value: undefined,
    };

    const newGroup: FilterGroup = {
      operator: 'and',
      conditions: [
        newCondition,
      ],
    };

    const newGroupForm = filterGroupForm(this.$form, newGroup);

    group.controls.conditions.push(newGroupForm);
  }

  addProperty() {
    const group = this.asFilterGroup(this.filter);

    const newCondition: FilterCondition = {
      property: null!,
      operator: 'equals',
      value: undefined,
    };

    const newConditionForm = filterConditionForm(this.$form, newCondition);

    group.controls.conditions.push(newConditionForm);
  }

  removeCondition(index: number) {
    const group = this.asFilterGroup(this.filter);

    group.controls.conditions.removeAt(index);
  }
}
