import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormArray, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { DefaultControlTypes, TypedFormGroup, formFactoryForModel } from '../../utilities/form/form.util';
import { FilterBuilderGroupComponent } from '../filter-builder-group/filter-builder-group.component';

export type FilterOperator = 'and' | 'or';

export interface FilterCondition {
  property: string;
  operator: FilterPropertyOperator;
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
  operators?: FilterPropertyOperator[];
}

export type FilterPropertyOperator = 'equals' | 'not_equals' | 'greater' | 'greater_equals'
  | 'less' | 'less_equals' | 'starts_with' | 'ends_with' | 'contains' | 'not_contains'
  | 'null' | 'not_null';

export const defaultOperators: { [key in FilterType]: FilterPropertyOperator[] } = {
  datetime: ['equals', 'not_equals', 'greater_equals', 'less_equals', 'null', 'not_null'],
  guid: ['equals', 'not_equals', 'null', 'not_null'],
  number: ['equals', 'not_equals', 'greater', 'greater_equals', 'less', 'less_equals', 'null', 'not_null'],
  string: ['equals', 'not_equals', 'starts_with', 'ends_with', 'contains', 'not_contains', 'null', 'not_null'],
}

export const showInput: { [key in FilterPropertyOperator]: boolean } = {
  contains: true,
  ends_with: true,
  equals: true,
  greater: true,
  greater_equals: true,
  less: true,
  less_equals: true,
  not_contains: true,
  not_equals: true,
  not_null: false,
  null: false,
  starts_with: true,
}

export const displayText: { [key in FilterType]: { [key in FilterPropertyOperator]: string } } = {
  datetime: {
    contains: '',
    ends_with: '',
    equals: 'At',
    greater: '',
    greater_equals: 'After',
    less: '',
    less_equals: 'Before',
    not_contains: '',
    not_equals: 'Not at',
    not_null: 'Is not null',
    null: 'Is null',
    starts_with: '',
  },
  guid: {
    contains: '',
    ends_with: '',
    equals: 'Equals',
    greater: '',
    greater_equals: '',
    less: '',
    less_equals: '',
    not_contains: '',
    not_equals: 'Does not equal',
    not_null: 'Is not null',
    null: 'Is null',
    starts_with: '',
  },
  number: {
    contains: '',
    ends_with: '',
    equals: 'Is equal to',
    greater: 'Greater than',
    greater_equals: 'Greater than or equal to',
    less: 'Less than',
    less_equals: 'Less than or equal to',
    not_contains: '',
    not_equals: 'Does not equal',
    not_null: 'Is not null',
    null: 'Is null',
    starts_with: '',
  },
  string: {
    contains: 'Contains',
    ends_with: 'Ends with',
    equals: 'Is equal to',
    greater: '',
    greater_equals: '',
    less: '',
    less_equals: '',
    not_contains: 'Does not contain',
    not_equals: 'Does not equal',
    not_null: 'Is not null',
    null: 'Is null',
    starts_with: 'Starts with',
  },
}

export function isFilterGroup(condition: FilterCondition | FilterGroup): condition is FilterGroup {
  return (condition as FilterGroup).conditions !== undefined;
}

export const filterGroupForm = formFactoryForModel<FilterGroup, DefaultControlTypes>(($form, model) => {
  return {
    operator: [model.operator, Validators.required],
    conditions: $form.nonNullable.array(model.conditions.map(item => {
      if (isFilterGroup(item)) {
        const subForm: TypedFormGroup<FilterGroup, DefaultControlTypes> = filterGroupForm($form, item);
        return subForm;
      } else {
        const subForm = filterConditionForm($form, item);
        return subForm;
      }
    })) as unknown as FormArray<TypedFormGroup<FilterGroup, DefaultControlTypes> | TypedFormGroup<FilterCondition, DefaultControlTypes>>,
  };
});

export const filterConditionForm = formFactoryForModel<FilterCondition, DefaultControlTypes>(($form: FormBuilder, model: FilterCondition) => {
  return {
    operator: [model.operator, Validators.required],
    property: [model.property, Validators.required],
    value: [model.value, Validators.required],
  };
});

export function isFilterGroupForm(condition: TypedFormGroup<FilterCondition, DefaultControlTypes> | TypedFormGroup<FilterGroup, DefaultControlTypes>): condition is TypedFormGroup<FilterGroup, DefaultControlTypes> {
  return (condition as TypedFormGroup<FilterGroup, DefaultControlTypes>).controls.conditions !== undefined;
}

@Component({
  selector: 'filter-builder',
  standalone: true,
  imports: [
    CommonModule,
    FilterBuilderGroupComponent,
  ],
  templateUrl: './filter-builder-modal.component.html',
  styleUrl: './filter-builder-modal.component.scss'
})
export class FilterBuilderModalComponent {

  readonly $activeModal: NgbActiveModal;
  private readonly $form: FormBuilder;

  filter?: TypedFormGroup<FilterGroup>;

  properties: FilterProperty[] = [];

  constructor(
    $activeModal: NgbActiveModal,
    $form: FormBuilder,
  ) {
    this.$activeModal = $activeModal;
    this.$form = $form;
  }

  setFilter(filter: FilterGroup, properties: FilterProperty[]) {
    this.filter = filterGroupForm(this.$form, filter);
    this.properties = properties;
  }

  trySubmit() {
    if (!this.filter)
      return;

    if (this.filter.invalid) {
      this.filter.markAllAsTouched();
      return;
    }

    this.$activeModal.close(this.filter.value);
  }
}
