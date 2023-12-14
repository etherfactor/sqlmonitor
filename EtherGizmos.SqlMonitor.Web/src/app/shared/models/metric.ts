import { DateTime } from "luxon";
import { z } from "zod";
import { Guid } from "../types/guid";
import { FormFunction, formFactoryForModel } from "../utilities/form/form.util";
import { AggregateType } from "./aggregate-type";
import { MetricSeverity, MetricSeverityDataZ, metricSeverityForm } from "./metric-severity";

export const MetricDataZ = z.object({
  id: z.string().uuid(),
  created_at: z.string().nullish(),
  created_by_user_id: z.string().uuid().nullish(),
  modified_at: z.string().nullish(),
  modified_by_user_id: z.string().uuid().nullish(),
  name: z.string(),
  description: z.string().nullish(),
  aggregate_type: z.nativeEnum(AggregateType),
  severities: z.lazy(() => z.array(MetricSeverityDataZ)),
});

export type MetricData = z.infer<typeof MetricDataZ>;

export type Metric = {
  id: Guid;
  createdAt?: DateTime;
  createdByUserId?: Guid;
  modifiedAt?: DateTime;
  modifiedByUserId?: Guid;
  name: string;
  description?: string;
  aggregateType?: AggregateType;
  severities: MetricSeverity[];
};

const metricFormFactory = formFactoryForModel<Metric, { id: 'control', createdAt: 'control', createdByUserId: 'control', modifiedAt: 'control', modifiedByUserId: 'control' }>(($form, model) => {
  return {
    id: [model.id],
    createdAt: [model.createdAt],
    createdByUserId: [model.createdByUserId],
    modifiedAt: [model.modifiedAt],
    modifiedByUserId: [model.modifiedByUserId],
    name: [model.name],
    description: [model.description],
    aggregateType: [model.aggregateType],
    severities: $form.nonNullable.array(model.severities.map(item => metricSeverityForm($form, item))),
  };
});

export const metricForm: FormFunction<Metric, { id: 'control', createdAt: 'control', createdByUserId: 'control', modifiedAt: 'control', modifiedByUserId: 'control' }> = function ($form, model) {
  if (!model)
    return undefined!;

  return metricFormFactory($form, model);
}
