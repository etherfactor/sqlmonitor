import { FormBuilder } from "@angular/forms";
import { GridStackWidget } from "gridstack";
import { ModelFormFunction, formFactoryForModel } from "../../../shared/utilities/form/form.util";

//==================================================
// Base widet
interface DashboardBaseWidget extends GridStackWidget {

  showOptions?: boolean;
}

//==================================================
// Chart widget

//X-axis
interface DashboardChartWidgetXAxis {

  label?: string;
  min?: number;
  minEnforced?: boolean;
  max?: number;
  maxEnforced?: boolean;
}

export interface DashboardChartWidgetLinearXAxis extends DashboardChartWidgetXAxis {

  type: 'linear';
}

const dashboardChartWidgetLinearXAxisFormFactory = formFactoryForModel((model: DashboardChartWidgetLinearXAxis) => {
  return {
    type: [model.type],
    label: [model.label],
    max: [model.max],
    maxEnforced: [model.maxEnforced],
    min: [model.min],
    minEnforced: [model.minEnforced],
  };
});

export const dashboardChartWidgetLinearXAxisForm: ModelFormFunction<DashboardChartWidgetLinearXAxis> = function($form: FormBuilder, model: DashboardChartWidgetLinearXAxis | undefined) {
  if (!model)
    return undefined!;

  return dashboardChartWidgetLinearXAxisFormFactory($form, model);
}

export interface DashboardChartWidgetLogarithmicXAxis extends DashboardChartWidgetXAxis {

  type: 'logarithmic';
}

export interface DashboardChartWidgetTimeXAxis extends DashboardChartWidgetXAxis {

  type: 'time';
  timeFormat?: string;
}

type DashboardLineChartWidgetXAxis =
  DashboardChartWidgetLinearXAxis |
  DashboardChartWidgetLogarithmicXAxis |
  DashboardChartWidgetTimeXAxis;

//Y-axis
interface DashboardChartWidgetYAxis extends DashboardChartWidgetXAxis {

  id: string;
}

export interface DashboardChartWidgetLinearYAxis extends DashboardChartWidgetLinearXAxis, DashboardChartWidgetYAxis {
  
}

export interface DashboardChartWidgetLogarithmicYAxis extends DashboardChartWidgetLogarithmicXAxis, DashboardChartWidgetYAxis {

}

export interface DashboardChartWidgetTimeYAxis extends DashboardChartWidgetTimeXAxis, DashboardChartWidgetYAxis {

}

type DashboardLineChartWidgetYAxis =
  DashboardChartWidgetLinearYAxis |
  DashboardChartWidgetLogarithmicYAxis |
  DashboardChartWidgetTimeYAxis;

//Chart widget
interface DashboardChartWidgetBase extends DashboardBaseWidget {

  type: 'chart';
}

interface DashboardLineChartWidget extends DashboardChartWidgetBase {

  chartType: 'linear';
  xAxis: DashboardLineChartWidgetXAxis;
  yAxis: DashboardLineChartWidgetYAxis[];
}

type DashboardChartWidget =
  DashboardLineChartWidget;

//==================================================
// Text widet
interface DashboardTextWidget extends DashboardBaseWidget {

  type: 'text';
  htmlContent?: string;
}

export type DashboardWidget =
  DashboardChartWidget |
  DashboardTextWidget;
