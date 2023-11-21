import { GridStackWidget } from "gridstack";

interface DashboardBaseWidget extends GridStackWidget {

  showOptions?: boolean;
}

interface DashboardChartWidget extends DashboardBaseWidget {

  type: 'chart';
}

interface DashboardTextWidget extends DashboardBaseWidget {

  type: 'text';
  htmlContent?: string;
}

export type DashboardWidget = DashboardChartWidget | DashboardTextWidget;
