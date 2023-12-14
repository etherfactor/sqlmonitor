import { z } from "zod";
import { FormFactoryMap, FormFunction, formFactoryForModel } from "../utilities/form/form.util";
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

const metricSeverityFormFactory = formFactoryForModel(($form, model: MetricSeverity) => {
  return <FormFactoryMap<MetricSeverity>>{
    severityType: [model.severityType],
    minimumValue: [model.minimumValue],
    maximumValue: [model.maximumValue],
  };
});

export const metricSeverityForm: FormFunction<MetricSeverity> = function ($form, model: MetricSeverity | undefined) {
  if (!model)
    return undefined!;

  return metricSeverityFormFactory($form, model);
}
