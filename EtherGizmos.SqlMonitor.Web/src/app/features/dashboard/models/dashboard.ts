import { z } from "zod";
import { GuidZ } from "../../../shared/types/guid/guid";
import { RelativeTimeZ } from "../../../shared/types/relative-time/relative-time";
import { DefaultControlTypes, formFactoryForModel } from "../../../shared/utilities/form/form.util";
import { DashboardWidgetZ, dashboardWidgetForm } from "./dashboard-widget";

export const DashboardZ = z.object({
  id: GuidZ,
  name: z.string(),
  description: z.string().optional(),
  timeStart: RelativeTimeZ,
  timeEnd: RelativeTimeZ,
  instanceIds: z.array(GuidZ),
  items: z.array(z.lazy(() => DashboardWidgetZ)),
});

export type Dashboard = z.infer<typeof DashboardZ>;

export const dashboardForm = formFactoryForModel<Dashboard, DefaultControlTypes>(($form, model) => {
  return {
    id: [model.id],
    name: [model.name],
    description: [model.description],
    timeStart: [model.timeStart],
    timeEnd: [model.timeEnd],
    instanceIds: $form.nonNullable.array(model.instanceIds.map(item => $form.nonNullable.control(item))),
    items: $form.nonNullable.array(model.items.map(item => dashboardWidgetForm($form, item))),
  };
});
