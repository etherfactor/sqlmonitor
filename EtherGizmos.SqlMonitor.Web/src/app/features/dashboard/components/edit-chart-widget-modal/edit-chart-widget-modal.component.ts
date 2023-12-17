import { CdkDrag, CdkDropList } from '@angular/cdk/drag-drop';
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { NgbAccordionModule, NgbActiveModal, NgbDropdownModule, NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { NgSelectModule } from '@ng-select/ng-select';
import { darken, lighten, readableColor, readableColorIsBlack, toRgba } from 'color2k';
import { ColorEvent } from 'ngx-color';
import { ColorSketchModule } from 'ngx-color/sketch';
import { TypedFormGroup } from '../../../../shared/utilities/form/form.util';
import { fromCamelCase } from '../../../../shared/utilities/string/string.util';
import { DashboardWidget, DashboardWidgetChartScaleType, DashboardWidgetChartType, dashboardWidgetForm } from '../../models/dashboard-widget';

@Component({
  selector: 'app-edit-chart-widget-modal',
  standalone: true,
  imports: [
    CdkDrag,
    CdkDropList,
    ColorSketchModule,
    CommonModule,
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

  chartTypes: { value: DashboardWidgetChartType, name: string }[] = [];
  scaleTypes: { value: DashboardWidgetChartScaleType, name: string }[] = [];

  color: string = '#00acac'; //rgb(0, 172, 172)

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

  get dynamicPickerStyle() {
    const base = this.color;
    const useLighten = !readableColorIsBlack(base);
    const text = readableColor(base);

    const hover = useLighten ? lighten(base, 0.05) : darken(base, 0.05);
    const active = useLighten ? lighten(base, 0.075) : darken(base, 0.075);
    const disabled = base;

    const shadowRgba = toRgba(base);
    const shadow = shadowRgba.substring(5, shadowRgba.length - 1)
      .replace(/ /g, '')
      .split(',')
      .slice(0, 3)
      .join(', ');

    return {
      '--bs-btn-color': text,
      '--bs-btn-bg': base,
      '--bs-btn-border-color': base,
      '--bs-btn-hover-color': text,
      '--bs-btn-hover-bg': hover,
      '--bs-btn-hover-border-color': hover,
      '--bs-btn-focus-shadow-rgb': shadow,
      '--bs-btn-active-color': text,
      '--bs-btn-active-bg': active,
      '--bs-btn-active-border-color': active,
      '--bs-btn-active-shadow': '',
      '--bs-btn-disabled-color': text,
      '--bs-btn-disabled-bg': disabled,
      '--bs-btn-disabled-border-color': disabled,
    };
  }

  colorChanged($event: ColorEvent) {
    this.color = $event.color.hex;
  }
}
