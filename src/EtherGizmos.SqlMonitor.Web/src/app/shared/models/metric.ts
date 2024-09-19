import { Validators } from "@angular/forms";
import { z } from "zod";
import { DateTimeZ } from "../types/datetime/datetime";
import { GuidZ } from "../types/guid/guid";
import { DefaultControlTypes, formFactoryForModel } from "../utilities/form/form.util";
import { AggregateType } from "./aggregate-type";

export const MetricZ = z.object({
  id: z.number().int(),
  createdAt: DateTimeZ,
  createdByUserId: GuidZ,
  modifiedAt: DateTimeZ.nullish(),
  modifiedByUserId: GuidZ.nullish(),
  name: z.string(),
  description: z.string().nullish(),
  aggregateType: z.nativeEnum(AggregateType),
  isActive: z.boolean(),
});

export interface Metric extends z.infer<typeof MetricZ> { }

export const metricForm = formFactoryForModel<Metric, DefaultControlTypes>(($form, model) => ({
  id: [model.id],
  createdAt: [model.createdAt],
  createdByUserId: [model.createdByUserId],
  modifiedAt: [model.modifiedAt],
  modifiedByUserId: [model.modifiedByUserId],
  name: [model.name, Validators.required],
  description: [model.description],
  aggregateType: [model.aggregateType, Validators.required],
  isActive: [model.isActive],
}));
