import { Validators } from "@angular/forms";
import { z } from "zod";
import { DefaultControlTypes, formFactoryForModel } from "../utilities/form/form.util";
import { SqlType } from "./sql-type";

export const QueryVariantZ = z.object({
  sqlType: z.nativeEnum(SqlType),
  queryText: z.string(),
});

export interface QueryVariant extends z.infer<typeof QueryVariantZ> { }

export const queryVariantForm = formFactoryForModel<QueryVariant, DefaultControlTypes>(($form, model) => ({
  sqlType: [model.sqlType, Validators.required],
  queryText: [model.queryText, Validators.required],
}));
