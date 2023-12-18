import { ControlConfig, FormArray, FormBuilder, FormControl, FormGroup, ɵElement } from "@angular/forms";

export function formFactoryForModel<TModel, TModify extends FormConfig<TModel> = undefined>(builder: ($form: FormBuilder, model: TModel) => Required<ControlConfigMap<TModel, TModify>>) {
  const result: ($form: FormBuilder, model: TModel) => TypedFormGroup<TModel, TModify> = ($form: FormBuilder, model: TModel) => {
    const config = builder($form, model);
    return $form.nonNullable.group(config);
  };

  return result;
}

type InferArrayType<TData> = TData extends (infer UData)[] ? UData : never;

export type FormConfig<TModel> = Partial<{ [K in keyof TModel]: 'control' | 'group' }> | undefined;

type ControlConfigMap<TModel, TModify extends FormConfig<TModel> = undefined> = {
  [K in keyof TModel]:
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  NonNullable<TModel[K]> extends Array<any> ? (
    K extends keyof TModify ? (
      TModify[K] extends 'control' ? ControlConfig<InferArrayType<TModel[K]>> :
      TModify[K] extends 'group' ? TypedFormGroup<InferArrayType<TModel[K]>> :
      undefined
    ) :
    InferArrayType<TModel[K]> extends object ?
    FormArray<TypedFormGroup<InferArrayType<TModel[K]>>> :
    FormArray<FormControl<InferArrayType<TModel[K]>>>
  ) :
  K extends keyof TModify ? (
    TModify[K] extends 'control' ? ControlConfig<TModel[K]> :
    TModify[K] extends 'group' ? TypedFormGroup<TModel[K]> :
    undefined
  ) :
  NonNullable<TModel[K]> extends object ? TypedFormGroup<TModel[K]> :
  ControlConfig<TModel[K]>;
};

export type FormFactoryMap<TModel, TModify extends FormConfig<TModel> = undefined> = Required<ControlConfigMap<TModel, TModify>>;

export type TypedFormGroup<TModel, TModify extends FormConfig<TModel> = undefined> = FormGroup<{
  [K in keyof Required<ControlConfigMap<TModel, TModify>>]: ɵElement<ControlConfigMap<TModel, TModify>[K], never>;
}>

const f = new FormBuilder();
f.nonNullable.array([f.nonNullable.group({
  a:[1]
})]);

export type FormFunction<TModel, TModify extends FormConfig<TModel> = undefined> = {
  ($form: FormBuilder, model: TModel): TypedFormGroup<TModel, TModify>;
  ($form: FormBuilder, model: TModel | undefined): TypedFormGroup<TModel, TModify> | undefined;
}

// eslint-disable-next-line @typescript-eslint/no-unused-vars
export function expectType<T>(_: T) {
  /* noop */
}
