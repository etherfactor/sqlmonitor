import { Component, forwardRef } from '@angular/core';
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from '@angular/forms';
import { BaseChartDirective } from 'ng2-charts';
import { generateGuid } from '../../../../shared/types/guid/guid';
import { ChartType } from '../edit-chart-widget-modal/edit-chart-widget-modal.component';

@Component({
  selector: 'edit-chart-widget-chart-selector',
  standalone: true,
  imports: [
    BaseChartDirective,
    FormsModule,
  ],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => EditChartWidgetChartSelectorComponent),
      multi: true,
    },
  ],
  templateUrl: './edit-chart-widget-chart-selector.component.html',
  styleUrl: './edit-chart-widget-chart-selector.component.scss'
})
export class EditChartWidgetChartSelectorComponent implements ControlValueAccessor {

  private onChange: (value: ChartType) => void = () => { };
  private onTouched: () => void = () => { };
  private isDisabled: boolean = false;

  value?: ChartType;

  unique = generateGuid();

  //Placeholder data for Line Chart
  public lineChartData = [
    { data: [65, 59, 80, 81, 56, 55, 40], label: 'Line Chart' }
  ];
  public lineChartLabels = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul'];

  //Placeholder data for Bar Chart
  public barChartData = [
    { data: [28, 48, 40, 19, 86, 27, 90], label: 'Bar Chart' }
  ];
  public barChartLabels = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul'];

  //Placeholder data for Pie Chart
  public pieChartData = [
    { data: [120, 150, 180, 90], label: 'Pie Chart' }
  ];
  public pieChartLabels = ['Red', 'Blue', 'Yellow', 'Green'];

  //Placeholder data for Scatter Chart
  public scatterChartData = [
    {
      data: [
        { x: 1, y: -1.1 },
        { x: 2, y: 1.4 },
        { x: 3, y: 2 },
        { x: 4, y: 1.5 },
        { x: 5, y: 0.5 },
        { x: 6, y: -0.2 },
      ],
      label: 'Scatter Dataset'
    }
  ];

  writeValue(value: ChartType): void {
    this.value = value;
  }

  registerOnChange(fn: (value: ChartType) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState?(isDisabled: boolean): void {
    this.isDisabled = isDisabled;
  }

  onChartClick(chartType: ChartType) {
    if (this.isDisabled)
      return;

    this.value = chartType;
    this.onChange(this.value);
    this.onTouched();
  }

  ChartType = ChartType;
}
