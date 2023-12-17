import { FormBuilder } from "@angular/forms";
import { GridStackWidget } from "gridstack";
import { z } from "zod";
import { FormFactoryMap, FormFunction, formFactoryForModel } from "../../../shared/utilities/form/form.util";

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

const dashboardWidgetChartScaleFormFactory = formFactoryForModel(($form, model: DashboardWidgetChartScale) => {
  return <FormFactoryMap<DashboardWidgetChartScale>> {
    id: [model.id],
    type: [model.type],
    label: [model.label],
    min: [model.min],
    minEnforced: [model.minEnforced],
    max: [model.max],
    maxEnforced: [model.maxEnforced],
    stacked: [model.stacked],
  };
});

export const dashboardWidgetChartScaleForm: FormFunction<DashboardWidgetChartScale> = function ($form: FormBuilder, model: DashboardWidgetChartScale | undefined) {
  if (!model)
    return undefined!;

  return dashboardWidgetChartScaleFormFactory($form, model);
};

// Chart
const DashboardWidgetChartZ = z.object({
  type: z.nativeEnum(DashboardWidgetChartType).optional(),
  colors: z.array(z.string()),
  xScale: DashboardWidgetChartScaleZ,
  yScales: z.array(DashboardWidgetChartScaleZ),
});

export type DashboardWidgetChart = z.infer<typeof DashboardWidgetChartZ>;

const dashboardWidgetChartFormFactory = formFactoryForModel(($form, model: DashboardWidgetChart) => {
  return <FormFactoryMap<DashboardWidgetChart>> {
    type: [model.type],
    colors: $form.nonNullable.array(model.colors),
    xScale: dashboardWidgetChartScaleForm($form, model.xScale),
    yScales: $form.nonNullable.array(model.yScales.map(item => dashboardWidgetChartScaleForm($form, item))),
  };
});

const dashboardWidgetChartForm: FormFunction<DashboardWidgetChart> = function ($form: FormBuilder, model: DashboardWidgetChart | undefined) {
  if (!model)
    return undefined!;

  return dashboardWidgetChartFormFactory($form, model);
}

//==================================================
// Text widget
const DashboardWidgetTextZ = z.object({
  htmlContent: z.string().optional(),
});

type DashboardWidgetText = z.infer<typeof DashboardWidgetTextZ>;

const dashboardWidgetTextFormFactory = formFactoryForModel(($form, model: DashboardWidgetText) => {
  return <FormFactoryMap<DashboardWidgetText>> {
    htmlContent: [model.htmlContent],
  };
});

const dashboardWidgetTextForm: FormFunction<DashboardWidgetText> = function ($form: FormBuilder, model: DashboardWidgetText | undefined) {
  if (!model)
    return undefined!;

  return dashboardWidgetTextFormFactory($form, model);
}

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

const dashboardWidgetGridFormFactory = formFactoryForModel(($form, model: DashboardWidgetGrid) => {
  return <FormFactoryMap<DashboardWidgetGrid>>{
    xPos: [model.xPos],
    yPos: [model.yPos],
    width: [model.width],
    height: [model.height],
    hovering: [model.hovering],
  };
});

const dashboardWidgetGridForm: FormFunction<DashboardWidgetGrid> = function ($form: FormBuilder, model: DashboardWidgetGrid | undefined) {
  if (!model)
    return undefined!;

  return dashboardWidgetGridFormFactory($form, model);
}

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

const dashboardWidgetFormFactory = formFactoryForModel(($form, model: DashboardWidget) => {
  return <FormFactoryMap<DashboardWidget>> {
    id: [model.id],
    type: [model.type],
    grid: dashboardWidgetGridForm($form, model.grid),
    chart: dashboardWidgetChartForm($form, model.chart),
    text: dashboardWidgetTextForm($form, model.text),
  };
});

export const dashboardWidgetForm: FormFunction<DashboardWidget> = function ($form: FormBuilder, model: DashboardWidget | undefined) {
  if (!model)
    return undefined!;

  return dashboardWidgetFormFactory($form, model);
}
