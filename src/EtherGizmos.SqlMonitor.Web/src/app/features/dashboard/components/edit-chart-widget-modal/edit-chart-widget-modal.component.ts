import { STEPPER_GLOBAL_OPTIONS } from '@angular/cdk/stepper';
import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatStepperModule } from '@angular/material/stepper';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Interval } from 'luxon';
import { Guid } from '../../../../shared/types/guid/guid';
import { RelativeTime } from '../../../../shared/types/relative-time/relative-time';

@Component({
  selector: 'edit-chart-widget-modal',
  standalone: true,
  imports: [
    MatStepperModule,
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
export class EditChartWidgetModalComponent {

  private readonly $activeModal: NgbActiveModal;
  private readonly $form: FormBuilder;

  firstFormGroup: FormGroup;
  secondFormGroup: FormGroup;

  constructor(
    $activeModal: NgbActiveModal,
    $form: FormBuilder,
  ) {
    this.$activeModal = $activeModal;
    this.$form = $form;

    this.firstFormGroup = this.$form.group({ firstCtrl: ['', Validators.required] });
    this.secondFormGroup = this.$form.group({ secondCtrl: [''] });
  }

  cancel() {
    this.$activeModal.dismiss();
  }

  trySubmit() {
    this.$activeModal.close();
  }

  ChartType = ChartType;
}

enum ChartType {
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

interface MetricConfiguration {
  metricId: number;
  yAxisIndex?: number;
  systems: {
    inherit: boolean;
    ids: Guid[];
  };
  resources: {
    inherit: boolean;
    ids: Guid[];
  };
  environments: {
    inherit: boolean;
    ids: Guid[];
  };
  tags: {
    inherit: boolean;
    values: TagConfiguration[];
  };
  buckets: {
    displayType: BucketDisplayType;
    aggregate: {
      type: AggregateType;
    };
    specific: {
      names: string[];
    };
    top: {
      count: number;
      remaining: {
        group: boolean;
        name: string;
      };
    };
  };
}

interface TagConfiguration {
  name: string;
  value: string;
}

interface TimeRangeConfiguration {
  inherit: boolean;
  startAt: RelativeTime;
  endAt: RelativeTime;
  aggregationInterval: Interval;
}

interface LegendConfiguration {
  show: boolean;
  position: PositionType;
}

interface AxisConfiguration {
  minimum?: number;
  minimumEnforced: boolean;
  maximum?: number;
  maximumEnforced: boolean;
  logarithmic: boolean;
  timeFormat?: string;
  label?: string;
  color?: string;
  stacked: boolean;
}

interface AppearanceConfiguration {
  defaultStyles: StyleConfiguration[];
  seriesStyles: SeriesStyleConfiguration[];
}

interface SeriesStyleConfiguration {
  metricId: number;
  bucketName?: string;
  style: StyleConfiguration;
}

interface StyleConfiguration {
  color?: string;
  lineWidth?: number;
  lineStyle?: LineStyleType;
  markerStyle?: MarkerStyleType;
  fill: {
    enabled: boolean;
    opacity: number;
  };
}
