import { Validators } from "@angular/forms";
import { z } from "zod";
import { DateTimeZ } from "../types/datetime/datetime";
import { GuidZ } from "../types/guid/guid";
import { DefaultControlTypes, formFactoryForModel } from "../utilities/form/form.util";

export const UserZ = z.object({
  id: GuidZ,
  createdAt: DateTimeZ,
  createdByUserId: GuidZ,
  modifiedAt: DateTimeZ.nullish(),
  modifiedByUserId: GuidZ.nullish(),
  name: z.string(),
  username: z.string(),
  password: z.string(),
  emailAddress: z.string().email().nullish(),
  isEmailValidated: z.boolean(),
  isActive: z.boolean(),
  lastLoginAt: DateTimeZ.nullish(),
  lastPasswordChangeAt: DateTimeZ,
});

export interface User extends z.infer<typeof UserZ> { }

export const userForm = formFactoryForModel<User, DefaultControlTypes>(($form, model) => {
  return {
    id: [model.id],
    createdAt: [model.createdAt],
    createdByUserId: [model.createdByUserId],
    modifiedAt: [model.modifiedAt],
    modifiedByUserId: [model.modifiedByUserId],
    name: [model.name, Validators.required],
    username: [model.username, Validators.required],
    password: [model.password],
    emailAddress: [model.emailAddress],
    isEmailValidated: [model.isEmailValidated],
    isActive: [model.isActive],
    lastLoginAt: [model.lastLoginAt],
    lastPasswordChangeAt: [model.lastPasswordChangeAt],
  };
});
