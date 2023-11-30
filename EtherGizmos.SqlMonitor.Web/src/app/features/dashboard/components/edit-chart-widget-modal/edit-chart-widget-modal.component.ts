import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TypedFormGroup } from '../../../../shared/utilities/form/form.util';
import { DashboardWidget, DashboardWidgetChartType, dashboardWidgetForm } from '../../models/dashboard-widget';
import { NgSelectModule } from '@ng-select/ng-select';
import { fromCamelCase } from '../../../../shared/utilities/string/string.util';

@Component({
  selector: 'app-edit-chart-widget-modal',
  standalone: true,
  imports: [
    CommonModule,
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

  chartTypes: { value: DashboardWidgetChartType, name: string }[] = undefined!;

  constructor(
    $activeModal: NgbActiveModal,
    $form: FormBuilder
  ) {
    this.$activeModal = $activeModal;
    this.$form = $form;
  }

  ngOnInit() {
    this.chartTypes = [];
    let values = Object.values(DashboardWidgetChartType);
    Object.keys(DashboardWidgetChartType).forEach((key, index) => {
      if (values[index] !== DashboardWidgetChartType.Bar && values[index] !== DashboardWidgetChartType.Line)
        return;

      this.chartTypes.push({
        value: values[index],
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
