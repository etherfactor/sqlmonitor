import { ControlConfig, FormArray, FormBuilder, FormControl, FormGroup, ɵElement } from "@angular/forms";
import { DateTime, Duration } from "luxon";
import { Guid } from "../../types/guid/guid";
import { RelativeTime } from "../../types/relative-time/relative-time";

export type DefaultControlTypes = DateTime | Duration | Guid | RelativeTime;

export function formFactoryForModel<TModel, TControlTypes = never>(builder: ($form: FormBuilder, model: TModel) => ControlConfigMap<TModel, TControlTypes>) {
  const result: FormFunction<TModel, TControlTypes> = ($form: FormBuilder, model: TModel | undefined): TypedFormGroup<TModel, TControlTypes> => {
    if (model === undefined || model === null)
      return undefined!;

    const config = builder($form, model);
    return $form.nonNullable.group(config);
  };

  return result;
}

type RequiredIsh<TType> = { [K in keyof Required<TType>]: TType[K]; };

export type InferArrayType<TData> = TData extends (infer UData)[] ? UData : never;

type NoUndefined<TType> = TType extends undefined ? never : TType;
type IfUndefined<TType> = TType extends undefined ? undefined : never;

type ControlConfigMap<TModel, TControlTypes = never> = RequiredIsh<{
  [K in keyof TModel]:
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  NonNullable<TModel[K]> extends Array<any> ? (
    InferArrayType<TModel[K]> extends (TControlTypes | undefined) ? (
      FormArray<FormControl<InferArrayType<TModel[K]>>>
    ) :
    InferArrayType<TModel[K]> extends object ?
    FormArray<TypedFormGroup<NoUndefined<InferArrayType<TModel[K]>>, TControlTypes>> :
    FormArray<FormControl<InferArrayType<TModel[K]>>>
  ) :
  TModel[K] extends (TControlTypes | undefined) ? (
    ControlConfig<TModel[K]>
  ) :
  NonNullable<TModel[K]> extends object ?
  TypedFormGroup<NoUndefined<TModel[K]>, TControlTypes> | IfUndefined<TModel[K]> :
  ControlConfig<TModel[K]>;
}>;

export type TypedFormGroup<TModel, TControlTypes = never> = FormGroup<{
  [K in keyof ControlConfigMap<TModel, TControlTypes>]: ɵElement<ControlConfigMap<TModel, TControlTypes>[K], never>;
}>

export type FormFunction<TModel, TControlTypes = never> = {
  ($form: FormBuilder, model: TModel): TypedFormGroup<TModel, TControlTypes>;
  ($form: FormBuilder, model: TModel | undefined): TypedFormGroup<TModel, TControlTypes> | undefined;
}

// eslint-disable-next-line @typescript-eslint/no-unused-vars
export function expectType<T>(_: T) {
  /* noop */
}

export function getAllFormValues<TModel, TControlTypes = never>(form: TypedFormGroup<TModel, TControlTypes>): TModel {
  if (form.invalid)
    throw new Error('Form is not valid');

  return form.getRawValue() as TModel;
}

export function getDirtyFormValues<TModel, TControlTypes = never>(form: TypedFormGroup<TModel, TControlTypes>): Partial<TModel> {
  if (form.invalid)
    throw new Error('Form is not valid');

  //Initialize empty object
  const dirtyValues = {} as Partial<TModel>;

  Object.keys(form.controls).forEach((key) => {

    const currentControl = form.controls[key as keyof typeof form.controls];

    if (currentControl.dirty) {
      //Check for nested controlGroups
      if (isFormGroupLike(currentControl)) {
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        dirtyValues[key as keyof typeof dirtyValues] = getDirtyFormValues(currentControl as FormGroup) as any; //TypeScript typing is a bit finicky here, but we know this is a valid value
      } else {
        dirtyValues[key as keyof typeof dirtyValues] = currentControl.value;
      }
    }
  });

  return dirtyValues;
}

function isFormGroupLike(value: unknown): value is FormGroup {
  return value instanceof FormGroup;
}
