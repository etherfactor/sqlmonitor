import { Validators } from "@angular/forms";
import { z } from "zod";
import { DefaultControlTypes, formFactoryForModel } from "../utilities/form/form.util";

export const ScriptVariantZ = z.object({
  scriptInterpreterId: z.number().int(),
  scriptText: z.string(),
});

export interface ScriptVariant extends z.infer<typeof ScriptVariantZ> { }

export const scriptVariantForm = formFactoryForModel<ScriptVariant, DefaultControlTypes>(($form, model) => ({
  scriptInterpreterId: [model.scriptInterpreterId, Validators.required],
  scriptText: [model.scriptText, Validators.required],
}));
