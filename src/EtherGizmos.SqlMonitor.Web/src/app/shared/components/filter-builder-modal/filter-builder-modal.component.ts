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

export type FilterType = 'boolean' | 'datetime' | 'guid' | 'number' | 'string';

export interface FilterProperty {
  name: string;
  displayName: string;
  type: FilterType;
  operators?: FilterPropertyOperator[];
}

export type FilterPropertyOperator = 'equals' | 'not_equals' | 'greater' | 'greater_equals'
  | 'less' | 'less_equals' | 'starts_with' | 'ends_with' | 'contains' | 'not_contains'
  | 'true' | 'false' | 'null' | 'not_null';

export const defaultOperators: { [key in FilterType]: FilterPropertyOperator[] } = {
  boolean: ['true', 'false', 'null', 'not_null'],
  datetime: ['equals', 'not_equals', 'greater_equals', 'less_equals', 'null', 'not_null'],
  guid: ['equals', 'not_equals', 'null', 'not_null'],
  number: ['equals', 'not_equals', 'greater', 'greater_equals', 'less', 'less_equals', 'null', 'not_null'],
  string: ['equals', 'not_equals', 'starts_with', 'ends_with', 'contains', 'not_contains', 'null', 'not_null'],
}

export const showInput: { [key in FilterPropertyOperator]: boolean } = {
  contains: true,
  ends_with: true,
  equals: true,
  false: false,
  greater: true,
  greater_equals: true,
  less: true,
  less_equals: true,
  not_contains: true,
  not_equals: true,
  not_null: false,
  null: false,
  starts_with: true,
  true: false,
}

export const displayText: { [key in FilterType]: { [key in FilterPropertyOperator]: string } } = {
  boolean: {
    contains: '',
    ends_with: '',
    equals: '',
    false: 'is false',
    greater: '',
    greater_equals: '',
    less: '',
    less_equals: '',
    not_contains: '',
    not_equals: '',
    not_null: 'is not null',
    null: 'is null',
    starts_with: '',
    true: 'is true',
  },
  datetime: {
    contains: '',
    ends_with: '',
    equals: 'at',
    false: '',
    greater: '',
    greater_equals: 'after',
    less: '',
    less_equals: 'before',
    not_contains: '',
    not_equals: 'not at',
    not_null: 'is not null',
    null: 'is null',
    starts_with: '',
    true: '',
  },
  guid: {
    contains: '',
    ends_with: '',
    equals: 'equals',
    false: '',
    greater: '',
    greater_equals: '',
    less: '',
    less_equals: '',
    not_contains: '',
    not_equals: 'does not equal',
    not_null: 'is not null',
    null: 'is null',
    starts_with: '',
    true: '',
  },
  number: {
    contains: '',
    ends_with: '',
    equals: 'is equal to',
    false: '',
    greater: 'greater than',
    greater_equals: 'greater than or equal to',
    less: 'less than',
    less_equals: 'less than or equal to',
    not_contains: '',
    not_equals: 'does not equal',
    not_null: 'is not null',
    null: 'is null',
    starts_with: '',
    true: '',
  },
  string: {
    contains: 'contains',
    ends_with: 'ends with',
    equals: 'is equal to',
    false: '',
    greater: '',
    greater_equals: '',
    less: '',
    less_equals: '',
    not_contains: 'does not contain',
    not_equals: 'does not equal',
    not_null: 'is not null',
    null: 'is null',
    starts_with: 'starts with',
    true: '',
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
