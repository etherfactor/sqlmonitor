import { z } from "zod";
import { DateTimeZ } from "../types/datetime/datetime";
import { GuidZ } from "../types/guid/guid";
import { DefaultControlTypes, formFactoryForModel } from "../utilities/form/form.util";

export const MonitoredSystemZ = z.object({
  id: GuidZ,
  createdAt: DateTimeZ,
  createdByUserId: GuidZ,
  modifiedAt: DateTimeZ.nullish(),
  modifiedByUserId: GuidZ.nullish(),
  name: z.string(),
  description: z.string().nullish(),
  isActive: z.boolean(),
});

export interface MonitoredSystem extends MonitoredSystem_Infer { }
type MonitoredSystem_Infer = z.infer<typeof MonitoredSystemZ>;

export const monitoredSystemForm = formFactoryForModel<MonitoredSystem, DefaultControlTypes>(($form, model) => {
  return {
    id: [model.id],
    createdAt: [model.createdAt],
    createdByUserId: [model.createdByUserId],
    modifiedAt: [model.modifiedAt],
    modifiedByUserId: [model.modifiedByUserId],
    name: [model.name],
    description: [model.description],
    isActive: [model.isActive],
  };
});
