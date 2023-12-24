import { Validators } from "@angular/forms";
import { GridStackWidget } from "gridstack";
import { z } from "zod";
import { DefaultControlTypes, formFactoryForModel } from "../../../shared/utilities/form/form.util";

//==================================================
// Enums
export enum DashboardWidgetType {
  Chart = "chart",
  Text = "text",
}

export enum DashboardWidgetChartType {
  Bar = "bar",
  Line = "line",
  Scatter = "scatter",
  Bubble = "bubble",
  Pie = "pie",
  Doughnut = "doughnut",
  PolarArea = "polarArea",
  Radar = "radar",
}

export enum DashboardWidgetChartScaleType {
  Linear = "linear",
  Logarithmic = "logarithmic",
  Time = "time",
  TimeSeries = "timeseries",
  RadialLinear = "radialLinear",
}

export enum DashboardWidgetChartMetricBucketType {
  Aggregate = "aggregate",
  SpecificBuckets = "specify",
  DisplayAll = "all",
  DisplayTopNCurrent = "topNCurrent",
  DisplayTopNRolling = "topNRolling",
}

//==================================================
// Chart widget

// Chart scale
export const DashboardWidgetChartScaleZ = z.object({
  id: z.string(),
  type: z.nativeEnum(DashboardWidgetChartScaleType),
  label: z.string().optional(),
  min: z.number().optional(),
  minEnforced: z.boolean().default(false),
  max: z.number().optional(),
  maxEnforced: z.boolean().default(false),
  stacked: z.boolean().default(false),
});

export type DashboardWidgetChartScale = z.infer<typeof DashboardWidgetChartScaleZ>;

export const dashboardWidgetChartScaleForm = formFactoryForModel<DashboardWidgetChartScale, DefaultControlTypes>(($form, model) => {
  return {
    id: [model.id, Validators.required],
    type: [model.type, Validators.required],
    label: [model.label],
    min: [model.min],
    minEnforced: [model.minEnforced],
    max: [model.max],
    maxEnforced: [model.maxEnforced],
    stacked: [model.stacked],
  };
});

// Chart metric
export const DashboardWidgetChartMetricZ = z.object({
  metricId: z.string(),
  yScaleId: z.string(),
  bucketType: z.nativeEnum(DashboardWidgetChartMetricBucketType),
  buckets: z.array(z.string()),
  bucketTopN: z.number().optional(),
});

export type DashboardWidgetChartMetric = z.infer<typeof DashboardWidgetChartMetricZ>;

export const dashboardWidgetChartMetricForm = formFactoryForModel<DashboardWidgetChartMetric, DefaultControlTypes>(($form, model) => {
  return {
    metricId: [model.metricId, Validators.required],
    yScaleId: [model.yScaleId, Validators.required],
    bucketType: [model.bucketType, Validators.required],
    buckets: $form.nonNullable.array(model.buckets),
    bucketTopN: [model.bucketTopN],
  };
});

// Chart
const DashboardWidgetChartZ = z.object({
  type: z.nativeEnum(DashboardWidgetChartType).optional(),
  colors: z.array(z.string()),
  xScale: DashboardWidgetChartScaleZ,
  yScales: z.array(DashboardWidgetChartScaleZ),
  metrics: z.array(DashboardWidgetChartMetricZ),
});

export type DashboardWidgetChart = z.infer<typeof DashboardWidgetChartZ>;

export const dashboardWidgetChartForm = formFactoryForModel<DashboardWidgetChart, DefaultControlTypes>(($form, model) => {
  return {
    type: [model.type, Validators.required],
    colors: $form.nonNullable.array(model.colors),
    xScale: dashboardWidgetChartScaleForm($form, model.xScale),
    yScales: $form.nonNullable.array(model.yScales.map(item => dashboardWidgetChartScaleForm($form, item))),
    metrics: $form.nonNullable.array(model.metrics.map(item => dashboardWidgetChartMetricForm($form, item))),
  };
});

//==================================================
// Text widget
const DashboardWidgetTextZ = z.object({
  htmlContent: z.string().optional(),
});

type DashboardWidgetText = z.infer<typeof DashboardWidgetTextZ>;

export const dashboardWidgetTextForm = formFactoryForModel<DashboardWidgetText, DefaultControlTypes>(($form, model) => {
  return {
    htmlContent: [model.htmlContent],
  };
});

//==================================================
// Grid configuration
const DashboardWidgetGridZ = z.object({
  xPos: z.number().int(),
  yPos: z.number().int(),
  width: z.number().int(),
  height: z.number().int(),
  hovering: z.boolean(),
});

type DashboardWidgetGrid = z.infer<typeof DashboardWidgetGridZ>;

export const dashboardWidgetGridForm = formFactoryForModel<DashboardWidgetGrid, DefaultControlTypes>(($form, model) => {
  return {
    xPos: [model.xPos],
    yPos: [model.yPos],
    width: [model.width],
    height: [model.height],
    hovering: [model.hovering],
  };
});

//==================================================
// Full widget
export const DashboardWidgetZ = z.object({
  id: z.string().uuid(),
  type: z.nativeEnum(DashboardWidgetType),
  grid: DashboardWidgetGridZ,
  chart: DashboardWidgetChartZ.optional(),
  text: DashboardWidgetTextZ.optional(),
});

export type DashboardWidget = z.infer<typeof DashboardWidgetZ>;

export interface GridstackDashboardWidget extends GridStackWidget {
  type: DashboardWidgetType,
  chart?: DashboardWidgetChart,
  text?: DashboardWidgetText,
}

export const dashboardWidgetForm = formFactoryForModel<DashboardWidget, DefaultControlTypes>(($form, model) => {
  return {
    id: [model.id],
    type: [model.type],
    grid: dashboardWidgetGridForm($form, model.grid),
    chart: dashboardWidgetChartForm($form, model.chart),
    text: dashboardWidgetTextForm($form, model.text),
  };
});
