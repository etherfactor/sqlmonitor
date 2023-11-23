import { ControlConfig, FormBuilder, FormGroup, ɵElement } from "@angular/forms";

export function formFactoryForModel<TModel>(builder: (model: TModel) => Required<ControlConfigMap<TModel>>) {
  const result: ($form: FormBuilder, model: TModel) => FormGroup<{
    [K in keyof Required<ControlConfigMap<TModel>>]: ɵElement<Required<ControlConfigMap<TModel>>[K], never>;
  }> = ($form: FormBuilder, model: TModel) => {
    const config = builder(model);
    return $form.nonNullable.group(config);
    //const cast: Required<ControlConfigMap<TModel>> = (base as unknown as Required<ControlConfigMap<TModel>>);
    //return $form.nonNullable.group(cast);
  };

  return result;
}

export type ControlConfigMap<TModel> = {
  [K in keyof TModel]: ControlConfig<TModel[K]>
};

export type TypedFormGroup<TModel> = FormGroup<{
  [K in keyof Required<ControlConfigMap<TModel>>]: ɵElement<Required<ControlConfigMap<TModel>>[K], never>;
}>

export type ModelFormFunction<TModel> = {
  ($form: FormBuilder, model: TModel): TypedFormGroup<TModel>;
  ($form: FormBuilder, model: TModel | undefined): TypedFormGroup<TModel> | undefined;
}
