import { Validators } from "@angular/forms";
import { z } from "zod";
import { DateTimeZ } from "../types/datetime/datetime";
import { DurationZ } from "../types/duration/duration";
import { GuidZ } from "../types/guid/guid";
import { DefaultControlTypes, formFactoryForModel } from "../utilities/form/form.util";
import { QueryMetricZ, queryMetricForm } from "./query-metric";
import { QueryVariantZ, queryVariantForm } from "./query-variant";

export const QueryZ = z.object({
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
  variants: z.lazy(() => QueryVariantZ).array(),
  metrics: z.lazy(() => QueryMetricZ).array(),
});

export interface Query extends z.infer<typeof QueryZ> { }

export const queryForm = formFactoryForModel<Query, DefaultControlTypes>(($form, model) => ({
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
  variants: $form.nonNullable.array(model.variants.sort((a, b) => a.sqlType.localeCompare(b.sqlType)).map(item => queryVariantForm($form, item))),
  metrics: $form.nonNullable.array(model.metrics.map(item => queryMetricForm($form, item))),
}));
