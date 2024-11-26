import { Validators } from "@angular/forms";
import { z } from "zod";
import { DateTimeZ } from "../types/datetime/datetime";
import { DurationZ } from "../types/duration/duration";
import { GuidZ } from "../types/guid/guid";
import { DefaultControlTypes, formFactoryForModel } from "../utilities/form/form.util";
import { ScriptMetricZ, scriptMetricForm } from "./script-metric";
import { ScriptVariantZ, scriptVariantForm } from "./script-variant";

export const ScriptZ = z.object({
  id: GuidZ,
  createdAt: DateTimeZ,
  createdByUserId: GuidZ,
  modifiedAt: DateTimeZ.nullish(),
  modifiedByUserId: GuidZ.nullish(),
  name: z.string(),
  description: z.string().nullish(),
  runFrequency: DurationZ,
  lastRunAt: DateTimeZ,
  lastFailureAt: DateTimeZ.nullish(),
  isActive: z.boolean(),
  bucketColumn: z.string().nullish(),
  timestampUtcColumn: z.string().nullish(),
  variants: z.lazy(() => ScriptVariantZ).array(),
  metrics: z.lazy(() => ScriptMetricZ).array(),
});

export interface Script extends z.infer<typeof ScriptZ> { }

export const scriptForm = formFactoryForModel<Script, DefaultControlTypes>(($form, model) => ({
  id: [model.id],
  createdAt: [model.createdAt],
  createdByUserId: [model.createdByUserId],
  modifiedAt: [model.modifiedAt],
  modifiedByUserId: [model.modifiedByUserId],
  name: [model.name, Validators.required],
  description: [model.description],
  runFrequency: [model.runFrequency],
  lastRunAt: [model.lastRunAt],
  lastFailureAt: [model.lastFailureAt],
  isActive: [model.isActive],
  bucketColumn: [model.bucketColumn],
  timestampUtcColumn: [model.timestampUtcColumn],
  variants: $form.nonNullable.array(model.variants.sort((a, b) => b.scriptInterpreterId - a.scriptInterpreterId).map(item => scriptVariantForm($form, item))),
  metrics: $form.nonNullable.array(model.metrics.map(item => scriptMetricForm($form, item))),
}));
