import { STEPPER_GLOBAL_OPTIONS } from '@angular/cdk/stepper';
import { Component, OnInit, computed, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatStepperModule } from '@angular/material/stepper';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NgSelectModule } from '@ng-select/ng-select';
import { Interval } from 'luxon';
import { EntitySearchDirective } from '../../../../shared/directives/entity-search/entity-search.directive';
import { Metric } from '../../../../shared/models/metric';
import { MetricService } from '../../../../shared/services/metric/metric.service';
import { Guid } from '../../../../shared/types/guid/guid';
import { RelativeTime } from '../../../../shared/types/relative-time/relative-time';
import { DefaultControlTypes, TypedFormGroup, formFactoryForModel, simpleForm } from '../../../../shared/utilities/form/form.util';
import { EntitySet, o } from '../../../../shared/utilities/odata/odata.util';
import { EditChartWidgetChartSelectorComponent } from '../edit-chart-widget-chart-selector/edit-chart-widget-chart-selector.component';

@Component({
  selector: 'edit-chart-widget-modal',
  standalone: true,
  imports: [
    EditChartWidgetChartSelectorComponent,
    EntitySearchDirective,
    MatStepperModule,
    NgSelectModule,
    ReactiveFormsModule,
  ],
  providers: [
    {
      provide: STEPPER_GLOBAL_OPTIONS,
      useValue: { showError: true },
    },
  ],
  templateUrl: './edit-chart-widget-modal.component.html',
  styleUrl: './edit-chart-widget-modal.component.scss'
})
export class EditChartWidgetModalComponent implements OnInit {

  private readonly $activeModal: NgbActiveModal;
  private readonly $form: FormBuilder;
  private readonly $metric: MetricService;

  firstFormGroup: FormGroup;
  secondFormGroup: FormGroup;

  step1Valid_type = signal(false);
  step1Valid = computed(() => {
    const result = this.step1Valid_type();
    return result;
  });

  formValid = computed(() => {
    const result = this.step1Valid();
    return result;
  });

  chartConfigurationForm!: TypedFormGroup<ChartConfiguration, DefaultControlTypes>;

  get metricSet() {
    return this.$metric.set;
  }

  constructor(
    $activeModal: NgbActiveModal,
    $form: FormBuilder,
    $metric: MetricService,
  ) {
    this.$activeModal = $activeModal;
    this.$form = $form;
    this.$metric = $metric;

    this.firstFormGroup = this.$form.group({ firstCtrl: ['', Validators.required] });
    this.secondFormGroup = this.$form.group({ secondCtrl: [''] });

    const chartConfig: ChartConfiguration = {
      chartType: undefined as unknown as ChartType,
      title: undefined as unknown as string,
      xAxis: {},
      yAxes: [{}],
      metrics: [],
      timeRange: {},
      appearance: {
        defaultStyles: [],
        seriesStyles: [],
      },
      legend: {},
    };
    this.initForm(chartConfig);
  }

  ngOnInit(): void {
  }

  initialize(chartConfig: ChartConfiguration) {
    this.initForm(chartConfig);
  }

  private initForm(chartConfig: ChartConfiguration) {
    this.chartConfigurationForm = chartConfigurationForm(this.$form, chartConfig);

    this.chartConfigurationForm.controls.chartType.valueChanges.subscribe(() => {
      this.step1Valid_type.set(!this.chartConfigurationForm.controls.chartType.invalid);
    });
  }

  cancel() {
    this.$activeModal.dismiss();
  }

  trySubmit() {
    console.log(this.chartConfigurationForm.value);
    this.$activeModal.close();
  }

  filterMetrics(term: string, entitySet: EntitySet<Metric>) {
    return entitySet.filter(e =>
      o.startsWith(
        e.prop('name'),
        o.string(term),
      ),
    );
  }

  ChartType = ChartType;
}

export enum ChartType {
  Line = "Line",
  Bar = "Bar",
  Pie = "Pie",
  Scatter = "Scatter",
}

enum AggregateType {
  Average = "Average",
  Maximum = "Maximum",
  Minimum = "Minimum",
  Sum = "Sum",
}

enum PositionType {
  Top = "Top",
  Bottom = "Bottom",
  Left = "Left",
  Right = "Right",
}

enum BucketDisplayType {
  Aggregate = "Aggregate",
  All = "All",
  Specific = "Specific",
  Top = "Top",
}

enum LineStyleType {
  Solid = "Solid",
  Dashed = "Dashed",
  Dotted = "Dotted",
}

enum MarkerStyleType {
  None = "None",
  Circle = "Circle",
  Square = "Square",
  Triangle = "Triangle",
}

interface ChartConfiguration {
  chartType: ChartType;
  xAxis: AxisConfiguration;
  yAxes: AxisConfiguration[];
  metrics: MetricConfiguration[];
  timeRange: TimeRangeConfiguration;
  appearance: AppearanceConfiguration;
  title?: string;
  legend: LegendConfiguration;
}

export const chartConfigurationForm = formFactoryForModel<ChartConfiguration, DefaultControlTypes>(($form, model) => ({
  chartType: [model.chartType, Validators.required],
  xAxis: axisConfigurationForm($form, model.xAxis),
  yAxes: $form.nonNullable.array(model.yAxes.map(item => axisConfigurationForm($form, item))),
  metrics: $form.nonNullable.array(model.metrics.map(item => metricConfigurationForm($form, item))),
  timeRange: timeRangeConfigurationForm($form, model.timeRange),
  appearance: appearanceConfigurationForm($form, model.appearance),
  title: [model.title],
  legend: legendConfigurationForm($form, model.legend),
}));

interface MetricConfiguration {
  metricId: number;
  yAxisIndex?: number;
  systems: {
    inherit?: boolean;
    ids: Guid[];
  };
  resources: {
    inherit?: boolean;
    ids: Guid[];
  };
  environments: {
    inherit?: boolean;
    ids: Guid[];
  };
  tags: {
    inherit?: boolean;
    values: TagConfiguration[];
  };
  buckets: {
    displayType: BucketDisplayType;
    aggregate: {
      type?: AggregateType;
    };
    specific: {
      names: string[];
    };
    top: {
      count: number;
      remaining: {
        group?: boolean;
        name?: string;
      };
    };
  };
}

export const metricConfigurationForm = formFactoryForModel<MetricConfiguration, DefaultControlTypes>(($form, model) => ({
  metricId: [model.metricId, Validators.required],
  yAxisIndex: [model.yAxisIndex],
  systems: simpleForm($form, model.systems, ($form, model) => ({
    inherit: [model.inherit],
    ids: $form.nonNullable.array(model.ids),
  })),
  resources: simpleForm($form, model.systems, ($form, model) => ({
    inherit: [model.inherit],
    ids: $form.nonNullable.array(model.ids),
  })),
  environments: simpleForm($form, model.environments, ($form, model) => ({
    inherit: [model.inherit],
    ids: $form.nonNullable.array(model.ids),
  })),
  tags: simpleForm($form, model.tags, ($form, model) => ({
    inherit: [model.inherit],
    values: $form.nonNullable.array(model.values.map(item => tagConfigurationForm($form, item)))
  })),
  buckets: simpleForm($form, model.buckets, ($form, model) => ({
    displayType: [model.displayType, Validators.required],
    aggregate: simpleForm($form, model.aggregate, ($form, model) => ({
      type: [model.type],
    })),
    specific: simpleForm($form, model.specific, ($form, model) => ({
      names: $form.nonNullable.array(model.names),
    })),
    top: simpleForm($form, model.top, ($form, model) => ({
      count: [model.count, Validators.required],
      remaining: simpleForm($form, model.remaining, ($form, model) => ({
        group: [model.group, Validators.required],
        name: [model.name],
      })),
    })),
  })),
}));

interface TagConfiguration {
  name: string;
  value: string;
}

const tagConfigurationForm = formFactoryForModel<TagConfiguration, DefaultControlTypes>(($form, model) => ({
  name: [model.name, Validators.required],
  value: [model.value, Validators.required],
}));

interface TimeRangeConfiguration {
  inherit?: boolean;
  startAt?: RelativeTime;
  endAt?: RelativeTime;
  aggregationInterval?: Interval;
}

const timeRangeConfigurationForm = formFactoryForModel<TimeRangeConfiguration, DefaultControlTypes>(($form, model) => ({
  inherit: [model.inherit, Validators.required],
  startAt: [model.startAt],
  endAt: [model.endAt],
  aggregationInterval: [model.aggregationInterval],
}));

interface LegendConfiguration {
  show?: boolean;
  position?: PositionType;
}

const legendConfigurationForm = formFactoryForModel<LegendConfiguration, DefaultControlTypes>(($form, model) => ({
  show: [model.show],
  position: [model.position],
}));

interface AxisConfiguration {
  minimum?: number;
  minimumEnforced?: boolean;
  maximum?: number;
  maximumEnforced?: boolean;
  logarithmic?: boolean;
  timeFormat?: string;
  label?: string;
  color?: string;
  stacked?: boolean;
}

const axisConfigurationForm = formFactoryForModel<AxisConfiguration, DefaultControlTypes>(($form, model) => ({
  minimum: [model.minimum],
  minimumEnforced: [model.minimumEnforced],
  maximum: [model.maximum],
  maximumEnforced: [model.maximumEnforced],
  logarithmic: [model.logarithmic],
  timeFormat: [model.timeFormat],
  label: [model.label],
  color: [model.color],
  stacked: [model.stacked],
}));

interface AppearanceConfiguration {
  defaultStyles: StyleConfiguration[];
  seriesStyles: SeriesStyleConfiguration[];
}

const appearanceConfigurationForm = formFactoryForModel<AppearanceConfiguration, DefaultControlTypes>(($form, model) => ({
  defaultStyles: $form.nonNullable.array(model.defaultStyles.map(item => styleConfigurationForm($form, item))),
  seriesStyles: $form.nonNullable.array(model.seriesStyles.map(item => seriesStyleConfigurationForm($form, item))),
}));

interface SeriesStyleConfiguration {
  metricId: number;
  bucketName?: string;
  style: StyleConfiguration;
}

const seriesStyleConfigurationForm = formFactoryForModel<SeriesStyleConfiguration, DefaultControlTypes>(($form, model) => ({
  metricId: [model.metricId, Validators.required],
  bucketName: [model.bucketName],
  style: styleConfigurationForm($form, model.style),
}));

interface StyleConfiguration {
  color?: string;
  lineWidth?: number;
  lineStyle?: LineStyleType;
  markerStyle?: MarkerStyleType;
  fill: {
    enabled?: boolean;
    opacity?: number;
  };
}

const styleConfigurationForm = formFactoryForModel<StyleConfiguration, DefaultControlTypes>(($form, model) => ({
  color: [model.color],
  lineWidth: [model.lineWidth],
  lineStyle: [model.lineStyle],
  markerStyle: [model.markerStyle],
  fill: simpleForm($form, model.fill, ($form, model) => ({
    enabled: [model.enabled],
    opacity: [model.opacity],
  })),
}));
