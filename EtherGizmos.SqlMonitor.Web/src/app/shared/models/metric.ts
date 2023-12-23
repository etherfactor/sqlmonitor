import { DateTime } from "luxon";
import { z } from "zod";
import { DateTimeZ, parseDateTime } from "../types/datetime/datetime";
import { Guid, GuidZ, parseGuid } from "../types/guid/guid";
import { DefaultControlTypes, expectType, formFactoryForModel } from "../utilities/form/form.util";
import { maybe } from "../utilities/maybe/maybe";
import { AggregateType } from "./aggregate-type";
import { MetricSeverity, MetricSeverityConverter, MetricSeverityDataZ, MetricSeverityZ, metricSeverityForm } from "./metric-severity";

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
  aggregateType: AggregateType;
  severities: MetricSeverity[];
};

export const MetricZ = z.object({
  id: GuidZ,
  createdAt: DateTimeZ.optional(),
  createdByUserId: GuidZ.optional(),
  modifiedAt: DateTimeZ.optional(),
  modifiedByUserId: GuidZ.optional(),
  name: z.string(),
  description: z.string().optional(),
  aggregateType: z.nativeEnum(AggregateType),
  severities: z.lazy(() => z.array(MetricSeverityZ)),
});

expectType<Metric>({} as z.infer<typeof MetricZ>);
expectType<z.infer<typeof MetricZ>>({} as Metric);

export class MetricConverter {
  static parse(input: unknown) {
    const data = MetricDataZ.parse(input);
    return this.fromData(data);
  }

  static fromData(input: MetricData): Metric {
    return {
      id: parseGuid(input.id),
      createdAt: maybe(() => parseDateTime(input.created_at)),
      createdByUserId: maybe(() => parseGuid(input.created_by_user_id)),
      modifiedAt: maybe(() => parseDateTime(input.modified_at)),
      modifiedByUserId: maybe(() => parseGuid(input.modified_by_user_id)),
      name: input.name,
      description: input.description ?? undefined,
      aggregateType: input.aggregate_type,
      severities: input.severities.map(item => MetricSeverityConverter.fromData(item)),
    };
  }

  static toCreate(input: Partial<Metric>): MetricData {
    const data = MetricZ.parse(input);
    return {
      id: data.id,
      created_at: data.createdAt?.toISO(),
      created_by_user_id: data.createdByUserId,
      modified_at: data.modifiedAt?.toISO(),
      modified_by_user_id: data.modifiedByUserId,
      name: data.name,
      description: data.description,
      aggregate_type: data.aggregateType,
      severities: data.severities.map(item => MetricSeverityConverter.toCreate(item)),
    };
  }

  static toPatch(input: Partial<Metric>): Partial<MetricData> {
    return {
      id: input.id,
      created_at: input.createdAt?.toISO(),
      created_by_user_id: input.createdByUserId,
      modified_at: input.modifiedAt?.toISO(),
      modified_by_user_id: input.modifiedByUserId,
      name: input.name,
      description: input.description,
      aggregate_type: input.aggregateType,
      severities: input.severities?.map(item => MetricSeverityConverter.toCreate(item)),
    };
  }
}

export const metricForm = formFactoryForModel<Metric, DefaultControlTypes>(($form, model) => {
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
