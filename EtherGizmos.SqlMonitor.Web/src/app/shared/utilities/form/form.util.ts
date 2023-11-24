import { ControlConfig, FormArray, FormBuilder, FormGroup, ɵElement } from "@angular/forms";

export function formFactoryForModel<TModel>(builder: ($form: FormBuilder, model: TModel) => Required<ControlConfigMap<TModel>>) {
  const result: ($form: FormBuilder, model: TModel) => FormGroup<{
    [K in keyof Required<ControlConfigMap<TModel>>]: ɵElement<Required<ControlConfigMap<TModel>>[K], never>;
  }> = ($form: FormBuilder, model: TModel) => {
    const config = builder($form, model);
    return $form.nonNullable.group(config);
    //const cast: Required<ControlConfigMap<TModel>> = (base as unknown as Required<ControlConfigMap<TModel>>);
    //return $form.nonNullable.group(cast);
  };

  return result;
}

type InferArrayType<TData> = TData extends (infer UData)[] ? UData : never;

export type ControlConfigMap<TModel> = {
  [K in keyof TModel]:
  TModel[K] extends Array<any> ? FormArray<ɵElement<InferArrayType<TModel[K]>, never>> :
  TModel[K] extends object ? TypedFormGroup<TModel[K]> :
  ControlConfig<TModel[K]>; // | FormArray<ɵElement<InferArrayType<TModel[K]>, never>>
};

export type TypedFormGroup<TModel> = FormGroup<{
  [K in keyof Required<ControlConfigMap<TModel>>]: ɵElement<Required<ControlConfigMap<TModel>>[K], never>;
}>

const f = new FormBuilder();
f.nonNullable.array([f.nonNullable.group({
  a:[1]
})]);

export type ModelFormFunction<TModel> = {
  ($form: FormBuilder, model: TModel): TypedFormGroup<TModel>;
  ($form: FormBuilder, model: TModel | undefined): TypedFormGroup<TModel> | undefined;
}

type TestMap<TModel> = {
  [K in keyof TModel]: TModel[K] extends Array<any> ? number : string;
}

interface TestInterface {
  simple: number;
  array: string[];
  data: {
    id: string;
    array: number[];
  }
}

formFactoryForModel(($form: FormBuilder, model: TestInterface) => {
  return {
    simple: [model.simple],
    array: $form.nonNullable.array<string>([]),
    data: $form.nonNullable.group({
      array: $form.nonNullable.array<number>([]),
      id: ['a' as string | undefined],
    }),
  };
});
