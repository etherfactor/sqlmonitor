import { DateTime } from "luxon";
import { z } from "zod";
import { DateTimeZ } from "../types/datetime/datetime";
import { Guid, GuidZ } from "../types/guid/guid";
import { expectType } from "../utilities/form/form.util";
import { SeverityType } from "./severity-type";

export const MetricDataDataZ = z.object({

});

export type MetricDataData = z.infer<typeof MetricDataDataZ>;

export type MetricData = {
  instanceId: Guid;
  metricId: Guid;
  eventTimeUtc: DateTime;
  severityType: SeverityType;
  bucket?: string;
  value: number;
};

const MetricDataZ = z.object({
  instanceId: GuidZ,
  metricId: GuidZ,
  eventTimeUtc: DateTimeZ,
  severityType: z.nativeEnum(SeverityType),
  bucket: z.string().optional(),
  value: z.number(),
});

expectType<MetricData>({} as z.infer<typeof MetricDataZ>);
expectType<z.infer<typeof MetricDataZ>>({} as MetricData);
