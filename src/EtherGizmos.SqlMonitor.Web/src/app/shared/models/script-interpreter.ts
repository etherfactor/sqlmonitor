import { Validators } from "@angular/forms";
import { z } from "zod";
import { DateTimeZ } from "../types/datetime/datetime";
import { GuidZ } from "../types/guid/guid";
import { DefaultControlTypes, formFactoryForModel } from "../utilities/form/form.util";

export const ScriptInterpreterZ = z.object({
  id: z.number().int(),
  createdAt: DateTimeZ,
  createdByUserId: GuidZ,
  modifiedAt: DateTimeZ.nullish(),
  modifiedByUserId: GuidZ.nullish(),
  name: z.string(),
  description: z.string().nullish(),
  command: z.string(),
  arguments: z.string(),
  extension: z.string(),
  //See https://microsoft.github.io/monaco-editor/
  monacoLanguage: z.string().nullish(),
});

export interface ScriptInterpreter extends z.infer<typeof ScriptInterpreterZ> { }

export const scriptInterpreterForm = formFactoryForModel<ScriptInterpreter, DefaultControlTypes>(($form, model) => ({
  id: [model.id],
  createdAt: [model.createdAt],
  createdByUserId: [model.createdByUserId],
  modifiedAt: [model.modifiedAt],
  modifiedByUserId: [model.modifiedByUserId],
  name: [model.name, Validators.required],
  description: [model.description],
  command: [model.command, Validators.required],
  arguments: [model.arguments, Validators.required],
  extension: [model.extension, Validators.required],
  monacoLanguage: [model.monacoLanguage],
}));
