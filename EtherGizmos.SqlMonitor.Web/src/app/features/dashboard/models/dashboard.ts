import { z } from "zod";
import { GuidZ } from "../../../shared/types/guid/guid";
import { RelativeTimeZ } from "../../../shared/types/relative-time/relative-time";
import { DefaultControlTypes, formFactoryForModel } from "../../../shared/utilities/form/form.util";
import { DashboardWidgetZ, dashboardWidgetForm } from "./dashboard-widget";

export const DashboardZ = z.object({
  id: GuidZ,
  timeStart: RelativeTimeZ,
  timeEnd: RelativeTimeZ,
  items: z.array(z.lazy(() => DashboardWidgetZ)),
});

export type Dashboard = z.infer<typeof DashboardZ>;

export const dashboardForm = formFactoryForModel<Dashboard, DefaultControlTypes>(($form, model) => {
  return {
    id: [model.id],
    timeStart: [model.timeStart],
    timeEnd: [model.timeEnd],
    items: $form.nonNullable.array(model.items.map(item => dashboardWidgetForm($form, item))),
  };
});
