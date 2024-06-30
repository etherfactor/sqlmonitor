import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ChartConfiguration, Point } from 'chart.js';
import { DateTime } from 'luxon';
import { BaseChartDirective } from 'ng2-charts';
import { Observable, Subject, Subscription, asyncScheduler, bufferTime, filter, first, interval, observeOn, shareReplay } from 'rxjs';
import { AggregateType } from '../../../../shared/models/aggregate-type';
import { Instance } from '../../../../shared/models/instance';
import { Metric } from '../../../../shared/models/metric';
import { MetricData } from '../../../../shared/models/metric-data';
import { MetricSubscription } from '../../../../shared/models/metric-subscription';
import { InstanceService } from '../../../../shared/services/instance/instance.service';
import { MetricDataCacheService } from '../../../../shared/services/metric-data-cache/metric-data-cache.service';
import { MetricDataService } from '../../../../shared/services/metric-data/metric-data.service';
import { MetricService } from '../../../../shared/services/metric/metric.service';
import { Guid, parseGuid } from '../../../../shared/types/guid/guid';
import { DashboardWidget, DashboardWidgetChartMetric, DashboardWidgetChartMetricBucketType, DashboardWidgetChartScaleType } from '../../models/dashboard-widget';
import { DeleteWidgetModalComponent } from '../delete-widget-modal/delete-widget-modal.component';
import { EditChartWidgetModalComponent } from '../edit-chart-widget-modal/edit-chart-widget-modal.component';

@Component({
  selector: 'chart-widget',
  standalone: true,
  imports: [
    CommonModule,
    BaseChartDirective,
  ],
  templateUrl: './chart-widget.component.html',
  styleUrl: './chart-widget.component.scss'
})
export class ChartWidgetComponent implements OnInit, OnChanges, OnDestroy {

  private $instance: InstanceService;
  private $metric: MetricService;
  private $metricData: MetricDataService;
  private $metricDataCache: MetricDataCacheService;
  private $modal: NgbModal;

  private metricSubscriptions: MetricSubscription[] = [];
  private rxjsSubscriptions: Subscription[] = [];
  private purgeSubscription?: Subscription;

  startTimeRetentionRevoke?: () => void;
  @Input({ required: true }) startTime: DateTime = undefined!;
  @Input({ required: true }) endTime: DateTime = undefined!;

  @Input({ required: true }) instances: Guid[] = [];

  @Input({ required: true }) config: DashboardWidget = undefined!;
  chartType: ChartConfiguration['type'] = undefined!;
  chartOptions: ChartConfiguration['options'] = {};
  chartData: ChartConfiguration['data'] = { datasets: [] };
  chartDatasets: { [datasetId: string]: ChartConfiguration['data']['datasets'][0] } = {};
  chartAggregators: { [datasetId: string]: MetricAggregator } = {};

  isHovering: boolean = false;

  instanceObservables: { [instanceId: Guid]: Observable<Instance> } = {};

  bufferSubject$ = new Subject<0>();
  bufferSubjectSubscription?: Subscription;

  @Output() onUpdate = new EventEmitter<DashboardWidget>();
  @Output() onDelete = new EventEmitter<DashboardWidget>();

  @ViewChild(BaseChartDirective) baseChart: BaseChartDirective = undefined!;

  constructor(
    $instance: InstanceService,
    $metric: MetricService,
    $metricData: MetricDataService,
    $metricDataCache: MetricDataCacheService,
    $modal: NgbModal
  ) {
    this.$instance = $instance;
    this.$metric = $metric;
    this.$metricData = $metricData;
    this.$metricDataCache = $metricDataCache;
    this.$modal = $modal;
  }

  ngOnInit(): void {
    this.updateWidget(this.config);

    this.purgeSubscription = interval(1000).pipe(
      observeOn(asyncScheduler),
    ).subscribe(() => this.purgeOldData());

    this.bufferSubjectSubscription = this.bufferSubject$.pipe(
      bufferTime(250),
      filter(array => array.length > 0),
    ).subscribe(() => this.baseChart.update());
  }

  ngOnChanges(changes: SimpleChanges): void {
    const startTimeChanges = changes['startTime'];
    if (startTimeChanges) {
      this.startTimeRetentionRevoke?.();
      this.startTimeRetentionRevoke = undefined;

      if (startTimeChanges.currentValue !== undefined && startTimeChanges.currentValue !== null) {
        this.startTimeRetentionRevoke = this.$metricDataCache.requestRetention(startTimeChanges.currentValue);
      }
    }

    const instancesChanges = changes['instances'];
    if (instancesChanges) {
      const datasetIds = Object.keys(this.chartDatasets);

      this.createChartData();
      for (const datasetId of datasetIds) {
        const dataset = this.chartDatasets[datasetId];
        const instanceId = parseGuid(datasetId.split('|')[1]);

        if (instancesChanges.currentValue.length === 0 || instancesChanges.currentValue.indexOf(instanceId) >= 0) {
          this.chartData.datasets.push(dataset);
        }
      }
    }
  }

  ngOnDestroy(): void {
    this.startTimeRetentionRevoke?.();
    this.flushSubscriptions();
    this.purgeSubscription?.unsubscribe();
    this.bufferSubjectSubscription?.unsubscribe();
  }

  editWidget() {
    const modal = this.$modal.open(EditChartWidgetModalComponent, { centered: true, size: 'lg', backdrop: 'static', keyboard: false });
    const modalInstance = <EditChartWidgetModalComponent>modal.componentInstance;
    modalInstance.setWidget(this.config);

    modal.result.then(
      (result: DashboardWidget) => {
        this.flushSubscriptions();

        this.updateWidget(result);
        this.onUpdate.next(result);
      },
      cancel => { }
    );
  }

  private getDatasetId(subscriptionId: Guid, instanceId: Guid, bucket: string | undefined) {
    return `${subscriptionId}|${instanceId}|${bucket ?? ''}`;
  }

  private getDataset(datasetId: string, label: string, yScaleId: string): ChartConfiguration['data']['datasets'][0] {
    if (this.chartDatasets[datasetId]) {
      return this.chartDatasets[datasetId];
    } else {
      const dataset = {
        label: label,
        xAxisID: 'x',
        yAxisID: yScaleId,
        data: [],
      };
      this.chartDatasets[datasetId] = dataset;

      const instanceId = parseGuid(datasetId.split('|')[1]);
      if (this.instances.length === 0 || this.instances.indexOf(instanceId) >= 0) {
        this.chartData.datasets.push(dataset);
      }

      return dataset;
    }
  }

  private getAggregator(datasetId: string, data: ChartConfiguration['data']['datasets'][0]['data'], aggregateType: AggregateType): MetricAggregator {
    let aggregator = this.chartAggregators[datasetId];
    if (!aggregator) {
      switch (aggregateType) {
        case (AggregateType.Average):
          aggregator = new AverageMetricAggregator(data);
          break;

        case (AggregateType.Maximum):
          aggregator = new MaximumMetricAggregator(data);
          break;

        case (AggregateType.Minimum):
          aggregator = new MinimumMetricAggregator(data);
          break;

        case (AggregateType.StandardDeviation):
          aggregator = new StandardDeviationMetricAggregator(data);
          break;

        case (AggregateType.Sum):
          aggregator = new SumMetricAggregator(data);
          break;

        case (AggregateType.Variance):
          aggregator = new VarianceMetricAggregator(data);
          break;

        default:
          throw new Error('Unknown aggregator');
      }
    }
    this.chartAggregators[datasetId] = aggregator;

    return aggregator;
  }

  private getInstanceObservable(instanceId: Guid): Observable<Instance> {
    return this.instanceObservables[instanceId] ?? this.$instance.get(instanceId).pipe(
      shareReplay({ bufferSize: 1, refCount: true }),
      first(),
    );
  }

  private updateWidget(newConfig: DashboardWidget) {
    this.flushSubscriptions();

    this.config = newConfig;
    this.createChartType();
    this.createChartOptions();
    this.createChartData();
    this.createChartDatasets();
    this.createChartAggregators();

    if (!newConfig.chart)
      return;

    for (const chartMetric of newConfig.chart.metrics) {
      this.$metric.get(chartMetric.metricId).subscribe(metric => {
        let chartMetricBuckets: (string | undefined)[] | undefined = undefined;
        if (chartMetric.bucketType === DashboardWidgetChartMetricBucketType.SpecificBuckets && chartMetric.buckets.length > 0) {
          chartMetricBuckets = chartMetric.buckets;
        }

        const sub = this.$metricData.subscribe(chartMetric.metricId, chartMetricBuckets);

        const historyData = this.$metricDataCache.get(chartMetric.metricId, chartMetricBuckets);
        historyData.sort((a, b) => {
          if (a.eventTimeUtc < b.eventTimeUtc)
            return -1;

          if (a.eventTimeUtc > b.eventTimeUtc)
            return 1;

          return 0;
        });

        for (const historyDatum of historyData) {
          this.getInstanceObservable(historyDatum.instanceId).subscribe(instance => {
            this.addData(chartMetric, sub, instance, metric, historyDatum);

            this.bufferSubject$.next(0);
          });
        }

        const rxjsSub = sub.data$.pipe(
          observeOn(asyncScheduler),
        ).subscribe(datum => {
          this.getInstanceObservable(datum.instanceId).subscribe(instance => {
            this.addData(chartMetric, sub, instance, metric, datum);

            this.bufferSubject$.next(0);
          });
        });
        this.metricSubscriptions.push(sub);
        this.rxjsSubscriptions.push(rxjsSub);
      });
    }
  }

  addData(chartMetric: DashboardWidgetChartMetric, subscription: MetricSubscription, instance: Instance, metric: Metric, datum: MetricData) {
    const label = this.getLabel(chartMetric, instance, metric, datum);
    if (chartMetric.bucketType === DashboardWidgetChartMetricBucketType.DisplayAll || chartMetric.bucketType === DashboardWidgetChartMetricBucketType.SpecificBuckets
      || chartMetric.bucketType === DashboardWidgetChartMetricBucketType.DisplayTopNCurrent || chartMetric.bucketType === DashboardWidgetChartMetricBucketType.DisplayTopNRolling) {
      const datasetId = this.getDatasetId(subscription.id, datum.instanceId, datum.bucket);
      const dataset = this.getDataset(datasetId, label, chartMetric.yScaleId);

      const datasetDatum = { x: datum.eventTimeUtc.toMillis(), y: datum.value };
      dataset.data.push(datasetDatum);
    } else if (chartMetric.bucketType === DashboardWidgetChartMetricBucketType.Aggregate) {
      const datasetId = this.getDatasetId(subscription.id, datum.instanceId, undefined);
      const dataset = this.getDataset(datasetId, label, chartMetric.yScaleId);

      const aggregateType = chartMetric.bucketAggregateType && chartMetric.bucketAggregateType !== AggregateType.Unknown
        ? chartMetric.bucketAggregateType
        : metric.aggregateType;

      const aggregator = this.getAggregator(datasetId, dataset.data, aggregateType);
      aggregator.addDatum(datum);
    } else {
      throw new Error('Unknown bucket type');
    }
  }

  getLabel(chartMetric: DashboardWidgetChartMetric, instance: Instance, metric: Metric, datum: MetricData) {
    let label = `[${instance.name}] ${chartMetric.label ?? metric.name}`;

    if (chartMetric.bucketType === DashboardWidgetChartMetricBucketType.DisplayAll || chartMetric.bucketType === DashboardWidgetChartMetricBucketType.SpecificBuckets
      || chartMetric.bucketType === DashboardWidgetChartMetricBucketType.DisplayTopNCurrent || chartMetric.bucketType === DashboardWidgetChartMetricBucketType.DisplayTopNRolling) {

      label += ` (${datum.bucket})`;

    } else if (chartMetric.bucketType === DashboardWidgetChartMetricBucketType.Aggregate) {

      const aggregateType = chartMetric.bucketAggregateType && chartMetric.bucketAggregateType !== AggregateType.Unknown
        ? chartMetric.bucketAggregateType
        : metric.aggregateType;

      switch (aggregateType) {
        case (AggregateType.Average):
          label += ' (Avg)';
          break;

        case (AggregateType.Maximum):
          label += ' (Max)';
          break;

        case (AggregateType.Minimum):
          label += ' (Min)';
          break;

        case (AggregateType.StandardDeviation):
          label += ' (StDev)';
          break;

        case (AggregateType.Sum):
          label += ' (Sum)';
          break;

        case (AggregateType.Variance):
          label += ' (Var)';
          break;

        default:
          throw new Error('Unknown aggregate type ' + chartMetric.bucketAggregateType ?? metric.aggregateType);
      }

    } else {
      throw new Error('Unknown bucket type');
    }

    return label;
  }

  deleteWidget() {
    this.$modal.open(DeleteWidgetModalComponent, { centered: true }).result.then(
      close => this.onDelete.next(this.config),
      cancel => { }
    );
  }

  flushSubscriptions() {
    for (const subscription of this.metricSubscriptions) {
      subscription.unsubscribe();
    }

    for (const subscription of this.rxjsSubscriptions) {
      subscription.unsubscribe();
    }
  }

  createChartType(): void {
    this.chartType = 'line';
  }

  createChartOptions(): void {
    const options: ChartConfiguration['options'] = {
      responsive: true,
      maintainAspectRatio: false,
      scales: {
        x: {
          axis: 'x',
          type: 'time',
          time: {
            unit: 'second',
            tooltipFormat: 'HH:mm:ss',
            displayFormats: {
              second: 'HH:mm:ss'
            },
          },
          ticks: {
            maxTicksLimit: 10,
          },
        },
      },
      ////Seems to cause some unknown error, TODO: figure the error out
      //parsing: false,
      //normalized: true,
      animation: false,
      plugins: {
        legend: { display: true },
        decimation: {
          enabled: true,
          algorithm: 'lttb',
          samples: 120,
        },
      },
    };

    if (!this.config.chart)
      throw new Error('Chart widget does not have chart configuration.');

    for (const scale of this.config.chart.yScales) {
      options.scales![scale.id] = {
        axis: 'y',
        type: scale.type === DashboardWidgetChartScaleType.Linear ? 'linear' : 'logarithmic',
        min: 0,
      };

      const newScale = options.scales![scale.id]!;

      if (scale.minEnforced) {
        newScale.min = scale.min;
      } else {
        newScale.suggestedMin = scale.min;
      }

      if (scale.maxEnforced) {
        newScale.max = scale.max;
      } else {
        newScale.suggestedMax = scale.max;
      }
    }

    this.chartOptions = options;
  }

  createChartData(): void {
    const data: ChartConfiguration['data'] = {
      datasets: [],
    };

    this.chartData = data;
  }

  createChartDatasets(): void {
    this.chartDatasets = {};
  }

  createChartAggregators(): void {
    this.chartAggregators = {};
  }

  private purgeOldData(): void {
    for (const datasetId of Object.keys(this.chartDatasets)) {
      const dataset = this.chartDatasets[datasetId];

      if (!dataset.data)
        continue;

      if (dataset.data.length === 0)
        continue;

      let isPastLimit = false;
      do {
        const datum = dataset.data[0];

        if (!datum) {
          dataset.data.shift();
          continue;
        }

        if (typeof datum !== 'object') {
          dataset.data.shift();
          continue;
        }

        if (Array.isArray(datum)) {
          dataset.data.shift();
          continue;
        }

        if (datum.x < this.startTime.toMillis()) {
          dataset.data.shift();
          continue;
        }

        isPastLimit = true;
      } while (!isPastLimit);
    }
  }
}

abstract class MetricAggregator {

  data: ChartConfiguration['data']['datasets'][0]['data'];

  constructor(data: ChartConfiguration['data']['datasets'][0]['data']) {
    this.data = data;
  }

  abstract addDatum(datum: MetricData): void;
}

export class AverageMetricAggregator extends MetricAggregator {

  aggregateData: { [key: number]: Point & { _ct: number } } = {};

  constructor(data: ChartConfiguration['data']['datasets'][0]['data']) {
    super(data);
  }

  override addDatum(datum: MetricData): void {
    const eventTime = datum.eventTimeUtc.toMillis();
    let aggregateDatum = this.aggregateData[eventTime];
    if (!aggregateDatum) {
      aggregateDatum = { x: eventTime, y: datum.value, _ct: 1 };
      this.aggregateData[eventTime] = aggregateDatum;
      this.data.push(aggregateDatum);
    } else {
      aggregateDatum.y = (aggregateDatum.y * aggregateDatum._ct + datum.value) / (aggregateDatum._ct + 1);
      aggregateDatum._ct++;
    }
  }
}

export class MaximumMetricAggregator extends MetricAggregator {

  aggregateData: { [key: number]: Point } = {};

  constructor(data: ChartConfiguration['data']['datasets'][0]['data']) {
    super(data);
  }

  override addDatum(datum: MetricData): void {
    const eventTime = datum.eventTimeUtc.toMillis();
    let aggregateDatum = this.aggregateData[eventTime];
    if (!aggregateDatum) {
      aggregateDatum = { x: eventTime, y: datum.value };
      this.aggregateData[eventTime] = aggregateDatum;
      this.data.push(aggregateDatum);
    } else {
      aggregateDatum.y = Math.max(aggregateDatum.y, datum.value);
    }
  }
}

export class MinimumMetricAggregator extends MetricAggregator {

  aggregateData: { [key: number]: Point } = {};

  constructor(data: ChartConfiguration['data']['datasets'][0]['data']) {
    super(data);
  }

  override addDatum(datum: MetricData): void {
    const eventTime = datum.eventTimeUtc.toMillis();
    let aggregateDatum = this.aggregateData[eventTime];
    if (!aggregateDatum) {
      aggregateDatum = { x: eventTime, y: datum.value };
      this.aggregateData[eventTime] = aggregateDatum;
      this.data.push(aggregateDatum);
    } else {
      aggregateDatum.y = Math.min(aggregateDatum.y, datum.value);
    }
  }
}

export class StandardDeviationMetricAggregator extends MetricAggregator {

  aggregateData: { [key: number]: Point & { _ct: number, _avg: number } } = {};

  constructor(data: ChartConfiguration['data']['datasets'][0]['data']) {
    super(data);
  }

  override addDatum(datum: MetricData): void {
    const eventTime = datum.eventTimeUtc.toMillis();
    let aggregateDatum = this.aggregateData[eventTime];
    if (!aggregateDatum) {
      aggregateDatum = { x: eventTime, y: 0, _ct: 1, _avg: datum.value };
      this.aggregateData[eventTime] = aggregateDatum;
      this.data.push(aggregateDatum);
    } else {
      const oldCount = aggregateDatum._ct;
      const newCount = aggregateDatum._ct + 1;

      const oldAverage = aggregateDatum._avg;
      const newAverage = (aggregateDatum._avg * aggregateDatum._ct + datum.value) / (newCount);

      aggregateDatum.y = Math.sqrt(
        (
          (oldCount) * aggregateDatum.y * aggregateDatum.y +
          (datum.value - newAverage) * (datum.value - oldAverage)
        ) / (newCount)
      );

      aggregateDatum._avg = newAverage;
      aggregateDatum._ct++;
    }
  }
}

export class SumMetricAggregator extends MetricAggregator {

  aggregateData: { [key: number]: Point } = {};

  constructor(data: ChartConfiguration['data']['datasets'][0]['data']) {
    super(data);
  }

  override addDatum(datum: MetricData): void {
    const eventTime = datum.eventTimeUtc.toMillis();
    let aggregateDatum = this.aggregateData[eventTime];
    if (!aggregateDatum) {
      aggregateDatum = { x: eventTime, y: datum.value };
      this.aggregateData[eventTime] = aggregateDatum;
      this.data.push(aggregateDatum);
    } else {
      aggregateDatum.y += datum.value;
    }
  }
}

export class VarianceMetricAggregator extends MetricAggregator {

  aggregateData: { [key: number]: Point & { _ct: number, _avg: number } } = {};

  constructor(data: ChartConfiguration['data']['datasets'][0]['data']) {
    super(data);
  }

  override addDatum(datum: MetricData): void {
    const eventTime = datum.eventTimeUtc.toMillis();
    let aggregateDatum = this.aggregateData[eventTime];
    if (!aggregateDatum) {
      aggregateDatum = { x: eventTime, y: 0, _ct: 1, _avg: datum.value };
      this.aggregateData[eventTime] = aggregateDatum;
      this.data.push(aggregateDatum);
    } else {
      const oldCount = aggregateDatum._ct;
      const newCount = aggregateDatum._ct + 1;

      const oldAverage = aggregateDatum._avg;
      const newAverage = (aggregateDatum._avg * aggregateDatum._ct + datum.value) / (newCount);

      aggregateDatum.y = (
        (
          (oldCount) * aggregateDatum.y +
          (datum.value - newAverage) * (datum.value - oldAverage)
        ) / (newCount)
      );

      aggregateDatum._avg = newAverage;
      aggregateDatum._ct++;
    }
  }
}
