import { ControlConfig, FormArray, FormBuilder, FormControl, FormGroup, ɵElement } from "@angular/forms";

export function formFactoryForModel<TModel>(builder: ($form: FormBuilder, model: TModel) => Required<ControlConfigMap<TModel>>) {
  const result: ($form: FormBuilder, model: TModel) => TypedFormGroup<TModel> = ($form: FormBuilder, model: TModel) => {
    const config = builder($form, model);
    return $form.nonNullable.group(config);
  };

  return result;
}

type InferArrayType<TData> = TData extends (infer UData)[] ? UData : never;

type ControlConfigMap<TModel> = {
  [K in keyof TModel]:
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  NonNullable<TModel[K]> extends Array<any> ? (
    InferArrayType<TModel[K]> extends object ?
    FormArray<TypedFormGroup<InferArrayType<TModel[K]>>> :
    FormArray<FormControl<InferArrayType<TModel[K]>>>
  ) :
  NonNullable<TModel[K]> extends object ? TypedFormGroup<TModel[K]> :
  ControlConfig<TModel[K]>;
};

export type FormFactoryMap<TModel> = Required<ControlConfigMap<TModel>>;

export type TypedFormGroup<TModel> = FormGroup<{
  [K in keyof Required<ControlConfigMap<TModel>>]: ɵElement<ControlConfigMap<TModel>[K], never>;
}>

const f = new FormBuilder();
f.nonNullable.array([f.nonNullable.group({
  a:[1]
})]);

export type FormFunction<TModel> = {
  ($form: FormBuilder, model: TModel): TypedFormGroup<TModel>;
  ($form: FormBuilder, model: TModel | undefined): TypedFormGroup<TModel> | undefined;
}
