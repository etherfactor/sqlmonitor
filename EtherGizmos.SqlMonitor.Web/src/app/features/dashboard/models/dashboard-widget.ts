import { FormBuilder } from "@angular/forms";
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
  xScale: DashboardWidgetChartScaleZ,
  yScales: z.array(DashboardWidgetChartScaleZ),
});

type DashboardWidgetChart = z.infer<typeof DashboardWidgetChartZ>;

const dashboardWidgetChartFormFactory = formFactoryForModel(($form, model: DashboardWidgetChart) => {
  return <FormFactoryMap<DashboardWidgetChart>> {
    type: [model.type],
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
// Full widget
export const DashboardWidgetZ = z.object({
  type: z.nativeEnum(DashboardWidgetType),
  chart: DashboardWidgetChartZ,
  text: DashboardWidgetTextZ,
});

export type DashboardWidget = z.infer<typeof DashboardWidgetZ>;

const dashboardWidgetFormFactory = formFactoryForModel(($form, model: DashboardWidget) => {
  return <FormFactoryMap<DashboardWidget>>{
    type: [model.type],
    chart: dashboardWidgetChartForm($form, model.chart),
    text: dashboardWidgetTextForm($form, model.text),
  };
});

export const dashboardWidgetForm: FormFunction<DashboardWidget> = function ($form: FormBuilder, model: DashboardWidget | undefined) {
  if (!model)
    return undefined!;

  return dashboardWidgetFormFactory($form, model);
}
