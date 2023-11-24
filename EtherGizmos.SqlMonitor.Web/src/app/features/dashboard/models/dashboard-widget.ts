import { FormBuilder } from "@angular/forms";
import { GridStackWidget } from "gridstack";
import { ModelFormFunction, formFactoryForModel } from "../../../shared/utilities/form/form.util";
import { FormElementArray, FormElementGroup, FormModel } from "ngx-mf";

//==================================================
// Base widet
interface DashboardBaseWidget extends GridStackWidget {

  showOptions?: boolean;
}

//==================================================
// Chart widget

//X-axis
interface DashboardChartWidgetXAxis {
  
  label?: string;
  min?: number;
  minEnforced?: boolean;
  max?: number;
  maxEnforced?: boolean;
}

export interface DashboardChartWidgetLinearXAxis extends DashboardChartWidgetXAxis {

  type: 'linear';
}

const dashboardChartWidgetLinearXAxisFormFactory = formFactoryForModel(($form: FormBuilder, model: DashboardChartWidgetLinearXAxis) => {
  return {
    type: [model.type],
    label: [model.label],
    max: [model.max],
    maxEnforced: [model.maxEnforced],
    min: [model.min],
    minEnforced: [model.minEnforced],
  };
});

export const dashboardChartWidgetLinearXAxisForm: ModelFormFunction<DashboardChartWidgetLinearXAxis> = function($form: FormBuilder, model: DashboardChartWidgetLinearXAxis | undefined) {
  if (!model)
    return undefined!;

  return dashboardChartWidgetLinearXAxisFormFactory($form, model);
}

export interface DashboardChartWidgetLogarithmicXAxis extends DashboardChartWidgetXAxis {

  type: 'logarithmic';
}

const dashboardChartWidgetLogarithmicXAxisFormFactory = formFactoryForModel(($form: FormBuilder, model: DashboardChartWidgetLogarithmicXAxis) => {
  return {
    type: [model.type],
    label: [model.label],
    max: [model.max],
    maxEnforced: [model.maxEnforced],
    min: [model.min],
    minEnforced: [model.minEnforced],
  };
});

export const dashboardChartWidgetLogarithmicXAxisForm: ModelFormFunction<DashboardChartWidgetLogarithmicXAxis> = function ($form: FormBuilder, model: DashboardChartWidgetLogarithmicXAxis | undefined) {
  if (!model)
    return undefined!;

  return dashboardChartWidgetLogarithmicXAxisFormFactory($form, model);
}

export interface DashboardChartWidgetTimeXAxis extends DashboardChartWidgetXAxis {

  type: 'time';
  timeFormat?: string;
}

const dashboardChartWidgetTimeXAxisFormFactory = formFactoryForModel(($form: FormBuilder, model: DashboardChartWidgetTimeXAxis) => {
  return {
    type: [model.type],
    label: [model.label],
    max: [model.max],
    maxEnforced: [model.maxEnforced],
    min: [model.min],
    minEnforced: [model.minEnforced],
    timeFormat: [model.timeFormat],
  };
});

export const dashboardChartWidgetTimeXAxisForm: ModelFormFunction<DashboardChartWidgetTimeXAxis> = function ($form: FormBuilder, model: DashboardChartWidgetTimeXAxis | undefined) {
  if (!model)
    return undefined!;

  return dashboardChartWidgetTimeXAxisFormFactory($form, model);
}

type DashboardLineChartWidgetXAxis =
  DashboardChartWidgetLinearXAxis |
  DashboardChartWidgetLogarithmicXAxis |
  DashboardChartWidgetTimeXAxis;

//Y-axis
interface DashboardChartWidgetYAxis extends DashboardChartWidgetXAxis {

  id: string;
}

export interface DashboardChartWidgetLinearYAxis extends DashboardChartWidgetLinearXAxis, DashboardChartWidgetYAxis {
  
}

function isLinearYAxis(item: DashboardChartWidgetYAxis): item is DashboardChartWidgetLinearYAxis {
  if ((item as DashboardChartWidgetLinearYAxis).type === "linear")
    return true;

  return false;
}

const dashboardChartWidgetLinearYAxisFormFactory = formFactoryForModel(($form: FormBuilder, model: DashboardChartWidgetLinearYAxis) => {
  return {
    type: [model.type],
    id: [model.id],
    label: [model.label],
    max: [model.max],
    maxEnforced: [model.maxEnforced],
    min: [model.min],
    minEnforced: [model.minEnforced],
  };
});

//export const dashboardChartWidgetLinearYAxisForm: ModelFormFunction<DashboardChartWidgetLinearYAxis> = function ($form: FormBuilder, model: DashboardChartWidgetLinearYAxis | undefined) {
//  if (!model)
//    return undefined!;

//  return dashboardChartWidgetLinearYAxisFormFactory($form, model);
//}

type FormModelControls<TModel extends {}, TRecord extends Record<string, any> | null = null> = FormModel<TModel, TRecord>['controls'];
type DashboardChartWidgetLinearYAxisForm = FormModelControls<DashboardChartWidgetLinearYAxis>;

export const dashboardChartWidgetLinearYAxisForm = function ($form: FormBuilder, model: DashboardChartWidgetLinearYAxis) {
  const nn = $form.nonNullable;
  return nn.group<DashboardChartWidgetLinearYAxisForm>({
    id: nn.control(model.id),
    label: nn.control(model.label),
    max: nn.control(model.max),
    maxEnforced: nn.control(model.maxEnforced),
    min: nn.control(model.min),
    minEnforced: nn.control(model.minEnforced),
    type: nn.control(model.type),
  });
}

export interface DashboardChartWidgetLogarithmicYAxis extends DashboardChartWidgetLogarithmicXAxis, DashboardChartWidgetYAxis {

}

function isLogarithmicYAxis(item: DashboardChartWidgetYAxis): item is DashboardChartWidgetLogarithmicYAxis {
  if ((item as DashboardChartWidgetLogarithmicYAxis).type === "logarithmic")
    return true;

  return false;
}

const dashboardChartWidgetLogarithmicYAxisFormFactory = formFactoryForModel(($form: FormBuilder, model: DashboardChartWidgetLogarithmicYAxis) => {
  return {
    type: [model.type],
    id: [model.id],
    label: [model.label],
    max: [model.max],
    maxEnforced: [model.maxEnforced],
    min: [model.min],
    minEnforced: [model.minEnforced],
  };
});

//export const dashboardChartWidgetLogarithmicYAxisForm: ModelFormFunction<DashboardChartWidgetLogarithmicYAxis> = function ($form: FormBuilder, model: DashboardChartWidgetLogarithmicYAxis | undefined) {
//  if (!model)
//    return undefined!;

//  return dashboardChartWidgetLogarithmicYAxisFormFactory($form, model);
//}

type DashboardChartWidgetLogarithmicYAxisForm = FormModelControls<DashboardChartWidgetLogarithmicYAxis>;

export const dashboardChartWidgetLogarithmicYAxisForm = function ($form: FormBuilder, model: DashboardChartWidgetLogarithmicYAxis) {
  const nn = $form.nonNullable;
  return nn.group<DashboardChartWidgetLogarithmicYAxisForm>({
    id: nn.control(model.id),
    label: nn.control(model.label),
    max: nn.control(model.max),
    maxEnforced: nn.control(model.maxEnforced),
    min: nn.control(model.min),
    minEnforced: nn.control(model.minEnforced),
    type: nn.control(model.type),
  });
}

export interface DashboardChartWidgetTimeYAxis extends DashboardChartWidgetTimeXAxis, DashboardChartWidgetYAxis {

}

function isTimeYAxis(item: DashboardChartWidgetYAxis): item is DashboardChartWidgetTimeYAxis {
  if ((item as DashboardChartWidgetTimeYAxis).type === "time")
    return true;

  return false;
}

const dashboardChartWidgetTimeYAxisFormFactory = formFactoryForModel(($form: FormBuilder, model: DashboardChartWidgetTimeYAxis) => {
  return {
    type: [model.type],
    id: [model.id],
    label: [model.label],
    max: [model.max],
    maxEnforced: [model.maxEnforced],
    min: [model.min],
    minEnforced: [model.minEnforced],
    timeFormat: [model.timeFormat],
  };
});

//export const dashboardChartWidgetTimeYAxisForm: ModelFormFunction<DashboardChartWidgetTimeYAxis> = function ($form: FormBuilder, model: DashboardChartWidgetTimeYAxis | undefined) {
//  if (!model)
//    return undefined!;

//  return dashboardChartWidgetTimeYAxisFormFactory($form, model);
//}

type DashboardChartWidgetTimeYAxisForm = FormModelControls<DashboardChartWidgetTimeYAxis>;

export const dashboardChartWidgetTimeYAxisForm = function ($form: FormBuilder, model: DashboardChartWidgetTimeYAxis) {
  const nn = $form.nonNullable;
  return nn.group<DashboardChartWidgetTimeYAxisForm>({
    id: nn.control(model.id),
    label: nn.control(model.label),
    max: nn.control(model.max),
    maxEnforced: nn.control(model.maxEnforced),
    min: nn.control(model.min),
    minEnforced: nn.control(model.minEnforced),
    timeFormat: nn.control(model.timeFormat),
    type: nn.control(model.type),
  });
}

type DashboardLineChartWidgetYAxis =
  DashboardChartWidgetLinearYAxis |
  DashboardChartWidgetLogarithmicYAxis |
  DashboardChartWidgetTimeYAxis;

//Chart widget
interface DashboardChartWidgetBase extends DashboardBaseWidget {

  type: 'chart';
}

interface DashboardLineChartWidget extends DashboardChartWidgetBase {

  chartType: 'linear';
  xAxis: DashboardLineChartWidgetXAxis;
  yAxes: DashboardLineChartWidgetYAxis[];
}

const dashboardLineChartWidgetFormFactory = formFactoryForModel(($form: FormBuilder, model: DashboardLineChartWidget) => {
  return {
    autoPosition: [model.autoPosition],
    chartType: [model.chartType],
    h: [model.h],
    htmlContent: [model.htmlContent],
    id: [model.id],
    locked: [model.locked],
    maxH: [model.maxH],
    maxW: [model.maxW],
    minH: [model.minH],
    minW: [model.minW],
    noMove: [model.noMove],
    noResize: [model.noResize],
    resizeToContentParent: [model.resizeToContentParent],
    showOptions: [model.showOptions],
    sizeToContent: [model.sizeToContent],
    subGridOpts: [model.subGridOpts],
    type: [model.type],
    w: [model.w],
    x: [model.x],
    xAxis: [model.xAxis],
    y: [model.y],
    yAxes: $form.nonNullable.array(model.yAxes.map(item => {
      if (isLinearYAxis(item))
        return dashboardChartWidgetLinearYAxisForm($form, item);

      if (isLogarithmicYAxis(item))
        return dashboardChartWidgetLogarithmicYAxisForm($form, item);

      if (isTimeYAxis(item))
        return dashboardChartWidgetTimeYAxisForm($form, item);

      throw new Error('Unexpected axis type');
    })),
  };
});

type test = FormModel<DashboardLineChartWidget>;
const model: DashboardLineChartWidget = {
  type: "chart",
  chartType: "linear",
  xAxis: {
    type: "time",
  },
  yAxes: [],
};
const f: FormBuilder = new FormBuilder();
f.group<FormModelControls<DashboardLineChartWidget, { yAxes: [FormElementGroup] }>>({
  autoPosition: f.nonNullable.control(model.autoPosition),
  chartType: f.nonNullable.control(model.chartType),
  h: f.nonNullable.control(model.h),
  htmlContent: f.nonNullable.control(model.htmlContent),
  id: f.nonNullable.control(model.id),
  locked: f.nonNullable.control(model.locked),
  maxH: f.nonNullable.control(model.maxH),
  maxW: f.nonNullable.control(model.maxW),
  minH: f.nonNullable.control(model.minH),
  minW: f.nonNullable.control(model.minW),
  noMove: f.nonNullable.control(model.noMove),
  noResize: f.nonNullable.control(model.noResize),
  resizeToContentParent: f.nonNullable.control(model.resizeToContentParent),
  showOptions: f.nonNullable.control(model.showOptions),
  sizeToContent: f.nonNullable.control(model.sizeToContent),
  subGridOpts: f.nonNullable.control(model.subGridOpts),
  type: f.nonNullable.control(model.type),
  w: f.nonNullable.control(model.w),
  x: f.nonNullable.control(model.x),
  xAxis: f.nonNullable.control(model.xAxis),
  y: f.nonNullable.control(model.x),
  yAxes: f.nonNullable.array<FormModelControls<DashboardLineChartWidgetYAxis>>([]),
});

interface zz {
  test: DashboardChartWidgetLinearXAxis[];
}

type zzForm = FormModelControls<zz, { test: [FormElementGroup] }>;

f.group<zzForm>({
  test: f.nonNullable.array<FormModelControls<DashboardChartWidgetLinearXAxis>>([]),
});

type ab = FormModelControls<DashboardLineChartWidget, { yAxes: [FormElementGroup] }>['yAxes'];

enum ContactType {
  Email,
  Telephone,
}

interface IContactModel {
  type: ContactType;
  contact: string;
}

interface IUserModel {
  id: number;
  firstName: string;
  lastName: string;
  nickname: string;
  birthday: number;
  contacts: IContactModel[];
}

const fb = new FormBuilder();

type UserForm = FormModel<Omit<IUserModel, 'id'>, { contacts: [FormElementGroup] }>;
type ContactForm = FormModel<IContactModel>;

const form: UserForm = fb.group<UserForm['controls']>({
  firstName: fb.nonNullable.control('test'),
  lastName: fb.nonNullable.control('test'),
  nickname: fb.nonNullable.control('test'),
  birthday: fb.nonNullable.control(13),
  contacts: fb.array<ContactForm>([].map(item => fb.group<ContactForm>(item))),
});

const value: Omit<IUserModel, 'id'> = {
  firstName: 'Vladislav',
  lastName: 'Lebedev',
  nickname: 'iam.guid',
  birthday: +new Date('022-07-07T17:10:03.102Z'),
  contacts: [{
    type: ContactType.Email,
    contact: 'iam.guid@gmail.com'
  }],
}

form.controls.contacts.controls.push(fb.group<ContactForm['controls']>({
  type: fb.nonNullable.control(ContactType.Email),
  contact: fb.nonNullable.control('test@test.com', Validators.email),
}));

form.patchValue(value);

expect(form.valid).toBeTruthy()
expect(form.value.contacts![0].contact).toBe('iam.guid@gmail.com');

const testValue: IUserModel = form.value as IUserModel;

export const dashboardLineChartWidgetForm: ModelFormFunction<DashboardLineChartWidget> = function ($form: FormBuilder, model: DashboardLineChartWidget | undefined) {
  if (!model)
    return undefined!;

  return dashboardLineChartWidgetFormFactory($form, model);
};

const a = dashboardLineChartWidgetForm($form, { type: "chart", chartType: "linear", xAxis: { type: "linear" }, yAxes: [] });
a.

type DashboardChartWidget =
  DashboardLineChartWidget;

//==================================================
// Text widet
interface DashboardTextWidget extends DashboardBaseWidget {

  type: 'text';
  htmlContent?: string;
}

export type DashboardWidget =
  DashboardChartWidget |
  DashboardTextWidget;
