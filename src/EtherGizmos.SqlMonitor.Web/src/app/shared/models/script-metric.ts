import { Validators } from "@angular/forms";
import { z } from "zod";
import { DefaultControlTypes, formFactoryForModel } from "../utilities/form/form.util";

export const ScriptMetricZ = z.object({
  metricId: z.number().int(),
  valueKey: z.string(),
});

export interface ScriptMetric extends z.infer<typeof ScriptMetricZ> { }

export const scriptMetricForm = formFactoryForModel<ScriptMetric, DefaultControlTypes>(($form, model) => ({
  metricId: [model.metricId, Validators.required],
  valueKey: [model.valueKey, Validators.required],
}));
