import { CdkDrag, CdkDropList } from '@angular/cdk/drag-drop';
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormsModule, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { NgbAccordionModule, NgbActiveModal, NgbDropdownModule, NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { NgSelectModule } from '@ng-select/ng-select';
import { ColorSketchModule } from 'ngx-color/sketch';
import { InputColorPickerComponent } from '../../../../shared/components/input-color-picker/input-color-picker.component';
import { Bound } from '../../../../shared/utilities/bound/bound.util';
import { TypedFormGroup } from '../../../../shared/utilities/form/form.util';
import { fromCamelCase } from '../../../../shared/utilities/string/string.util';
import { ColorSet } from '../../models/color-set';
import { DashboardWidget, DashboardWidgetChartScaleType, DashboardWidgetChartType, dashboardWidgetChartScaleForm, dashboardWidgetForm } from '../../models/dashboard-widget';

@Component({
  selector: 'app-edit-chart-widget-modal',
  standalone: true,
  imports: [
    CdkDrag,
    CdkDropList,
    ColorSketchModule,
    CommonModule,
    FormsModule,
    InputColorPickerComponent,
    NgbAccordionModule,
    NgbDropdownModule,
    NgbPaginationModule,
    NgSelectModule,
    ReactiveFormsModule,
  ],
  templateUrl: './edit-chart-widget-modal.component.html',
  styleUrl: './edit-chart-widget-modal.component.scss'
})
export class EditChartWidgetModalComponent implements OnInit {

  private $form: FormBuilder;

  $activeModal: NgbActiveModal;

  widget?: DashboardWidget;
  widgetForm?: TypedFormGroup<DashboardWidget>;

  get chartForm() { return this.widgetForm?.controls?.chart; }

  chartTypes: { value: DashboardWidgetChartType, name: string }[] = [];
  scaleTypes: { value: DashboardWidgetChartScaleType, name: string }[] = [];

  colorSets: ColorSet[] = [
    {
      name: 'Custom',
      colors: [],
    },
    {
      name: 'Primary',
      colors: ['#8a3ffc', '#33b1ff', '#007d79', '#ff7eb6', '#fa4d56', '#fff1f1', '#6fdc8c', '#4589ff'],
    },
    {
      name: 'Secondary',
      colors: ['#e60049', '#0bb4ff', '#50e991', '#e6d800', '#9b19f5', '#ffa300', '#dc0ab4', '#b3d4ff'],
    }
  ];
  colorSet: ColorSet = this.colorSets[0];
  colors: string[] = [...this.colorSet.colors];

  newYScaleForm: FormControl<string> = undefined!;

  constructor(
    $activeModal: NgbActiveModal,
    $form: FormBuilder
  ) {
    this.$activeModal = $activeModal;
    this.$form = $form;
  }

  ngOnInit() {
    const chartValues = Object.values(DashboardWidgetChartType);
    Object.keys(DashboardWidgetChartType).forEach((key, index) => {
      if (chartValues[index] !== DashboardWidgetChartType.Bar && chartValues[index] !== DashboardWidgetChartType.Line)
        return;

      this.chartTypes.push({
        value: chartValues[index],
        name: fromCamelCase(key),
      });
    });

    const scaleValues = Object.values(DashboardWidgetChartScaleType);
    Object.keys(DashboardWidgetChartScaleType).forEach((key, index) => {
      if (scaleValues[index] !== DashboardWidgetChartScaleType.Linear && scaleValues[index] !== DashboardWidgetChartScaleType.Logarithmic)
        return;

      this.scaleTypes.push({
        value: scaleValues[index],
        name: fromCamelCase(key),
      });
    });

    this.newYScaleForm = this.$form.nonNullable.control<string>(
      undefined!,
      {
        validators: [
          Validators.required,
          this.validateUniqueYScaleKey,
        ]
      });
  }

  setWidget(item: DashboardWidget) {
    this.widget = item;
    this.widgetForm = dashboardWidgetForm(this.$form, item);
  }

  trySubmit() {
    if (!this.widgetForm)
      return;

    console.log(this.widgetForm.value);

    this.$activeModal.close(this.widgetForm.value);
  }

  changeColorSet(colorSet?: ColorSet) {
    this.colorSet = colorSet ?? this.colorSets[0];
    if (this.chartForm && this.chartForm.controls) {
      for (let i = 0; i < this.colorSet.colors.length; i++) {
        this.chartForm.controls.colors.setControl(i, this.$form.nonNullable.control(this.colorSet.colors[i]));
      }
    }
  }

  addYScale() {
    if (!this.chartForm)
      return;

    if (!this.chartForm.controls)
      return;

    if (!this.newYScaleForm.valid) {
      this.newYScaleForm.markAllAsTouched();
      return;
    }

    const newYScaleKey = this.newYScaleForm.value;
    this.chartForm.controls.yScales.push(dashboardWidgetChartScaleForm(this.$form, {
      id: newYScaleKey,
      type: DashboardWidgetChartScaleType.Linear,
      minEnforced: false,
      maxEnforced: false,
      stacked: false,
    }));

    this.newYScaleForm = this.$form.nonNullable.control<string>(
      undefined!,
      {
        validators: [
          Validators.required,
          this.validateUniqueYScaleKey,
        ]
      });
  }

  @Bound validateUniqueYScaleKey(control: FormControl<string>): ValidationErrors | null {
    if (!this.chartForm)
      return null;

    if (!this.chartForm.controls)
      return null;

    for (const yScale of this.chartForm.controls.yScales.controls) {
      if (control.value?.toLowerCase() === yScale.value.id?.toLowerCase()) {
        return { error: true };
      }
    }

    return null;
  }
}
