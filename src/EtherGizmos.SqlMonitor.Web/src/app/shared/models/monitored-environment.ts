import { Validators } from "@angular/forms";
import { z } from "zod";
import { DateTimeZ } from "../types/datetime/datetime";
import { GuidZ } from "../types/guid/guid";
import { DefaultControlTypes, formFactoryForModel } from "../utilities/form/form.util";

export const MonitoredEnvironmentZ = z.object({
  id: GuidZ,
  createdAt: DateTimeZ,
  createdByUserId: GuidZ,
  modifiedAt: DateTimeZ.nullish(),
  modifiedByUserId: GuidZ.nullish(),
  name: z.string(),
  description: z.string().nullish(),
  isActive: z.boolean(),
});

export interface MonitoredEnvironment extends z.infer<typeof MonitoredEnvironmentZ> { }

export const monitoredEnvironmentForm = formFactoryForModel<MonitoredEnvironment, DefaultControlTypes>(($form, model) => {
  return {
    id: [model.id],
    createdAt: [model.createdAt],
    createdByUserId: [model.createdByUserId],
    modifiedAt: [model.modifiedAt],
    modifiedByUserId: [model.modifiedByUserId],
    name: [model.name, Validators.required],
    description: [model.description],
    isActive: [model.isActive, Validators.required],
  };
});
