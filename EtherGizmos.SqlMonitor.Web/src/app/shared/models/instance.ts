import { DateTime } from "luxon";
import { z } from "zod";
import { DateTimeZ, parseDateTime } from "../types/datetime/datetime";
import { Guid, GuidZ, parseGuid } from "../types/guid/guid";
import { expectType } from "../utilities/form/form.util";
import { maybe } from "../utilities/maybe/maybe.util";

export const InstanceDataZ = z.object({
  id: z.string().uuid(),
  created_at: z.string().nullish(),
  created_by_user_id: z.string().uuid().nullish(),
  modified_at: z.string().nullish(),
  modified_by_user_id: z.string().uuid().nullish(),
  name: z.string(),
  description: z.string().nullish(),
  is_active: z.boolean(),
  address: z.string(),
  port: z.number().int().nullish(),
  database: z.string().nullish(),
});

export type InstanceData = z.infer<typeof InstanceDataZ>;

export type Instance = {
  id: Guid;
  createdAt?: DateTime;
  createdByUserId?: Guid;
  modifiedAt?: DateTime;
  modifiedByUserId?: Guid;
  name: string;
  description?: string;
  isActive: boolean;
  address: string;
  port?: number;
  database?: string;
};

export const InstanceZ = z.object({
  id: GuidZ,
  createdAt: DateTimeZ.optional(),
  createdByUserId: GuidZ.optional(),
  modifiedAt: DateTimeZ.optional(),
  modifiedByUserId: GuidZ.optional(),
  name: z.string(),
  description: z.string().optional(),
  isActive: z.boolean(),
  address: z.string(),
  port: z.number().int().optional(),
  database: z.string().optional(),
});

expectType<Instance>({} as z.infer<typeof InstanceZ>);
expectType<z.infer<typeof InstanceZ>>({} as Instance);

export class InstanceConverter {
  static parse(input: unknown) {
    const data = InstanceDataZ.parse(input);
    return this.fromData(data);
  }

  static fromData(input: InstanceData): Instance {
    return {
      id: parseGuid(input.id),
      createdAt: maybe(() => parseDateTime(input.created_at)),
      createdByUserId: maybe(() => parseGuid(input.created_by_user_id)),
      modifiedAt: maybe(() => parseDateTime(input.modified_at)),
      modifiedByUserId: maybe(() => parseGuid(input.modified_by_user_id)),
      name: input.name,
      description: input.description ?? undefined,
      isActive: input.is_active,
      address: input.address,
      port: input.port ?? undefined,
      database: input.database ?? undefined,
    };
  }

  static toCreate(input: Instance): InstanceData {
    return {
      id: undefined!,
      created_at: undefined,
      created_by_user_id: undefined,
      modified_at: undefined,
      modified_by_user_id: undefined,
      name: input.name,
      description: input.description,
      is_active: input.isActive,
      address: input.address,
      port: input.port,
      database: input.database,
    };
  }

  static toPatch(input: Instance): InstanceData {
    return {
      id: undefined!,
      created_at: undefined,
      created_by_user_id: undefined,
      modified_at: undefined,
      modified_by_user_id: undefined,
      name: input.name,
      description: input.description,
      is_active: input.isActive,
      address: input.address,
      port: input.port,
      database: input.database,
    };
  }
}
