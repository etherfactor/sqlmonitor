import { z } from "zod";
import { DefaultControlTypes, expectType, formFactoryForModel } from "../utilities/form/form.util";
import { SeverityType } from "./severity-type";

export const MetricSeverityDataZ = z.object({
  severity_type: z.nativeEnum(SeverityType),
  minimum_value: z.number().nullish(),
  maximum_value: z.number().nullish(),
});

export type MetricSeverityData = z.infer<typeof MetricSeverityDataZ>;

export type MetricSeverity = {
  severityType: SeverityType;
  minimumValue?: number;
  maximumValue?: number;
};

export const MetricSeverityZ = z.object({
  severityType: z.number(),
  minimumValue: z.number().optional(),
  maximumValue: z.number().optional(),
});

expectType<MetricSeverity>({} as z.infer<typeof MetricSeverityZ>);
expectType<z.infer<typeof MetricSeverityZ>>({} as MetricSeverity);

export class MetricSeverityConverter {
  static parse(input: unknown) {
    const data = MetricSeverityDataZ.parse(input);
    return this.fromData(data);
  }

  static fromData(input: MetricSeverityData): MetricSeverity {
    return {
      severityType: input.severity_type,
      minimumValue: input.minimum_value ?? undefined,
      maximumValue: input.maximum_value ?? undefined,
    };
  }

  static toCreate(input: Partial<MetricSeverity>): MetricSeverityData {
    const data = MetricSeverityZ.parse(input);
    return {
      severity_type: data.severityType,
      minimum_value: data.minimumValue,
      maximum_value: data.maximumValue,
    };
  }

  static toPatch(input: Partial<MetricSeverity>): Partial<MetricSeverityData> {
    return {
      severity_type: input.severityType,
      minimum_value: input.minimumValue,
      maximum_value: input.maximumValue,
    };
  }
}

export const metricSeverityForm = formFactoryForModel<MetricSeverity, DefaultControlTypes>(($form, model) => {
  return {
    severityType: [model.severityType],
    minimumValue: [model.minimumValue],
    maximumValue: [model.maximumValue],
  };
});
