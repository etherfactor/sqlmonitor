import { ControlConfig, FormArray, FormBuilder, FormControl, FormGroup, ɵElement } from "@angular/forms";
import { DateTime, Duration } from "luxon";
import { Guid } from "../../types/guid/guid";

export type DefaultControlTypes = DateTime | Duration | Guid;

export function formFactoryForModelOld<TModel>(builder: ($form: FormBuilder, model: TModel) => Required<ControlConfigMap<TModel>>) {
  const result: ($form: FormBuilder, model: TModel) => TypedFormGroup<TModel> = ($form: FormBuilder, model: TModel) => {
    const config = builder($form, model);
    return $form.nonNullable.group(config);
  };

  return result;
}

export function formFactoryForModel<TModel, TControlTypes = never>(builder: ($form: FormBuilder, model: TModel) => ControlConfigMap<TModel, TControlTypes>) {
  const result: FormFunction<TModel, TControlTypes> = ($form: FormBuilder, model: TModel | undefined): TypedFormGroup<TModel, TControlTypes> => {
    if (model === undefined || model === null)
      return undefined!;

    const config = builder($form, model);
    return $form.nonNullable.group(config);
  };

  return result;
}

type InferArrayType<TData> = TData extends (infer UData)[] ? UData : never;

type ControlConfigMap<TModel, TControlTypes = never> = Required<{
  [K in keyof TModel]:
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  NonNullable<TModel[K]> extends Array<any> ? (
    InferArrayType<TModel[K]> extends (TControlTypes | undefined) ? (
      FormArray<FormControl<InferArrayType<TModel[K]>>>
    ) :
    InferArrayType<TModel[K]> extends object ?
    FormArray<TypedFormGroup<InferArrayType<TModel[K]>, TControlTypes>> :
    FormArray<FormControl<InferArrayType<TModel[K]>>>
  ) :
  TModel[K] extends (TControlTypes | undefined) ? (
    ControlConfig<TModel[K]>
  ) :
  NonNullable<TModel[K]> extends object ?
  TypedFormGroup<TModel[K], TControlTypes> :
  ControlConfig<TModel[K]>;
}>;

//export type FormFactoryMap<TModel> = Required<ControlConfigMap<TModel>>;

export type TypedFormGroup<TModel, TControlTypes = never> = FormGroup<{
  [K in keyof Required<ControlConfigMap<TModel, TControlTypes>>]: ɵElement<ControlConfigMap<TModel, TControlTypes>[K], never>;
}>

export type FormFunction<TModel, TControlTypes = never> = {
  ($form: FormBuilder, model: TModel): TypedFormGroup<TModel, TControlTypes>;
  ($form: FormBuilder, model: TModel | undefined): TypedFormGroup<TModel, TControlTypes> | undefined;
}

// eslint-disable-next-line @typescript-eslint/no-unused-vars
export function expectType<T>(_: T) {
  /* noop */
}
