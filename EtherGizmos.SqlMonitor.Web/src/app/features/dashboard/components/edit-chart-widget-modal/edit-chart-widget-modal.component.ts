import { CdkDrag, CdkDropList } from '@angular/cdk/drag-drop';
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { NgbAccordionModule, NgbActiveModal, NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { NgSelectModule } from '@ng-select/ng-select';
import { TypedFormGroup } from '../../../../shared/utilities/form/form.util';
import { fromCamelCase } from '../../../../shared/utilities/string/string.util';
import { DashboardWidget, DashboardWidgetChartScaleType, DashboardWidgetChartType, dashboardWidgetForm } from '../../models/dashboard-widget';

@Component({
  selector: 'app-edit-chart-widget-modal',
  standalone: true,
  imports: [
    CdkDrag,
    CdkDropList,
    CommonModule,
    NgbAccordionModule,
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
}
