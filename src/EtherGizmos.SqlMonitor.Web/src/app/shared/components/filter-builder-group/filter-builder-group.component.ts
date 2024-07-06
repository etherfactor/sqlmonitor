import { CommonModule } from '@angular/common';
import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { NgxMaskDirective } from 'ngx-mask';
import { Subscription } from 'rxjs';
import { DefaultControlTypes, TypedFormGroup } from '../../utilities/form/form.util';
import { FilterCondition, FilterGroup, FilterProperty, FilterPropertyOperator, defaultOperators, displayText, filterConditionForm, filterGroupForm, isFilterGroupForm, showInput } from '../filter-builder-modal/filter-builder-modal.component';
import { InputLuxonDatetimeComponent } from '../input-luxon-datetime/input-luxon-datetime.component';

@Component({
  selector: 'filter-builder-group',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InputLuxonDatetimeComponent,
    NgSelectModule,
    NgxMaskDirective,
    ReactiveFormsModule,
  ],
  templateUrl: './filter-builder-group.component.html',
  styleUrl: './filter-builder-group.component.scss'
})
export class FilterBuilderGroupComponent implements OnChanges {

  private readonly $form: FormBuilder;

  @Input() root: boolean = false;

  @Input({ required: true }) filter!: TypedFormGroup<FilterCondition, DefaultControlTypes> | TypedFormGroup<FilterGroup, DefaultControlTypes>;

  @Input({ required: true }) properties!: FilterProperty[];

  private filterSubscriptions: Subscription[] = [];

  guidPatterns = {
    'X': { pattern: /[0-9A-Fa-f]/ },
    '4': { pattern: /4/ },
    '8': { pattern: /[8-9A-Ba-b]/ }
  };

  constructor(
    $form: FormBuilder,
  ) {
    this.$form = $form;
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['filter']) {
      for (const sub of this.filterSubscriptions) {
        sub.unsubscribe();
      }
      this.filterSubscriptions = [];

      const newFilter = changes['filter'].currentValue as TypedFormGroup<FilterCondition, DefaultControlTypes> | TypedFormGroup<FilterGroup, DefaultControlTypes>;
      if (!isFilterGroupForm(newFilter)) {
        const propertySub = newFilter.controls.property.valueChanges.subscribe(() => {
          const operator = newFilter.controls.operator;
          operator.setValue(undefined);
          operator.updateValueAndValidity();
        });
        this.filterSubscriptions.push(propertySub);

        const operatorSub = newFilter.controls.operator.valueChanges.subscribe(operator => {
          const value = newFilter.controls.value;
          value.setValue(undefined);

          if (showInput[operator ?? 'equals']) {
            value.setValidators([Validators.required]);
          } else {
            value.setValidators([]);
          }

          value.updateValueAndValidity();
        });
        this.filterSubscriptions.push(operatorSub);
      }
    }
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

  showOperatorValue() {
    const condition = this.asFilterCondition(this.filter);
    return showInput[condition.controls.operator.value ?? '' as FilterPropertyOperator] ?? false;
  }

  addGroup() {
    const group = this.asFilterGroup(this.filter);

    const newCondition: FilterCondition = {
      property: undefined!,
      operator: undefined!,
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
      property: undefined!,
      operator: undefined!,
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
