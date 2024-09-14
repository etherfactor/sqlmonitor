import { Validators } from "@angular/forms";
import { z } from "zod";
import { DefaultControlTypes, formFactoryForModel } from "../utilities/form/form.util";

export const QueryMetricZ = z.object({
  metricId: z.number().int(),
  valueColumn: z.string(),
});

export interface QueryMetric extends z.infer<typeof QueryMetricZ> { }

export const queryMetricForm = formFactoryForModel<QueryMetric, DefaultControlTypes>(($form, model) => ({
  metricId: [model.metricId, Validators.required],
  valueColumn: [model.valueColumn, Validators.required],
}));
