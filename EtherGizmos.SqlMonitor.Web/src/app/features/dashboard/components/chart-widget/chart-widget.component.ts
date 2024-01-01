import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ChartConfiguration } from 'chart.js';
import { DateTime } from 'luxon';
import { BaseChartDirective, NgChartsModule } from 'ng2-charts';
import { Subscription } from 'rxjs';
import { MetricSubscription } from '../../../../shared/models/metric-subscription';
import { MetricDataCacheService } from '../../../../shared/services/metric-data-cache/metric-data-cache.service';
import { MetricDataService } from '../../../../shared/services/metric-data/metric-data.service';
import { Guid } from '../../../../shared/types/guid/guid';
import { DashboardWidget, DashboardWidgetChartMetricBucketType, DashboardWidgetChartScaleType } from '../../models/dashboard-widget';
import { DeleteWidgetModalComponent } from '../delete-widget-modal/delete-widget-modal.component';
import { EditChartWidgetModalComponent } from '../edit-chart-widget-modal/edit-chart-widget-modal.component';

@Component({
  selector: 'chart-widget',
  standalone: true,
  imports: [
    CommonModule,
    NgChartsModule,
  ],
  templateUrl: './chart-widget.component.html',
  styleUrl: './chart-widget.component.scss'
})
export class ChartWidgetComponent implements OnInit {

  private $metricData: MetricDataService;
  private $metricDataCache: MetricDataCacheService;
  private $modal: NgbModal;

  private metricSubscriptions: MetricSubscription[] = [];
  private rxjsSubscriptions: Subscription[] = [];

  @Input({ required: true }) config: DashboardWidget = undefined!;
  chartType: ChartConfiguration['type'] = undefined!;
  chartOptions: ChartConfiguration['options'] = undefined!;
  chartData: ChartConfiguration['data'] = undefined!;
  chartDatasets: { [datasetId: string]: ChartConfiguration['data']['datasets'][0] } = {};

  @Output() onUpdate = new EventEmitter<DashboardWidget>();
  @Output() onDelete = new EventEmitter<DashboardWidget>();

  @ViewChild(BaseChartDirective) baseChart: BaseChartDirective = undefined!;

  constructor(
    $metricData: MetricDataService,
    $metricDataCache: MetricDataCacheService,
    $modal: NgbModal
  ) {
    this.$metricData = $metricData;
    this.$metricDataCache = $metricDataCache;
    this.$modal = $modal;
  }

  ngOnInit(): void {
    this.updateWidget(this.config);
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

  private getDatasetId(subscriptionId: Guid, bucket: string | undefined) {
    return `${subscriptionId}|${bucket ?? ''}`;
  }

  private getDataset(datasetId: string, metricId: Guid, yScaleId: string): ChartConfiguration['data']['datasets'][0] {
    if (this.chartDatasets[datasetId]) {
      return this.chartDatasets[datasetId];
    } else {
      const dataset = {
        label: metricId,
        xAxisID: 'x',
        yAxisID: yScaleId,
        data: [],
      };
      this.chartData.datasets.push(dataset);
      this.chartDatasets[datasetId] = dataset;

      return dataset;
    }
  }

  private updateWidget(newConfig: DashboardWidget) {
    this.config = newConfig;
    this.createChartType();
    this.createChartOptions();
    this.createChartData();
    this.createChartDatasets();

    if (!newConfig.chart)
      return;

    for (const chartMetric of newConfig.chart.metrics) {
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
        const datasetId = this.getDatasetId(sub.id, historyDatum.bucket);
        const dataset = this.getDataset(datasetId, historyDatum.metricId, chartMetric.yScaleId);

        dataset.data.push({ x: historyDatum.eventTimeUtc.toMillis(), y: historyDatum.value });
      }
      this.baseChart.update();

      const rxjsSub = sub.data$.subscribe(datum => {
        const datasetId = this.getDatasetId(sub.id, datum.bucket);
        const dataset = this.getDataset(datasetId, datum.metricId, chartMetric.yScaleId);

        dataset.data.push({ x: datum.eventTimeUtc.toMillis(), y: datum.value });

        this.purgeOldData();

        this.baseChart.update();
      });
      this.metricSubscriptions.push(sub);
      this.rxjsSubscriptions.push(rxjsSub);
    }
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
      plugins: {
        legend: { display: true },
        decimation: {
          enabled: true,
          algorithm: 'min-max',
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

  private purgeOldData(): void {
    const now = DateTime.utc();

    for (const datasetId of Object.keys(this.chartDatasets)) {
      const dataset = this.chartDatasets[datasetId];

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

        if (now.diff(DateTime.fromMillis(datum.x)).as('minutes') > 1) {
          dataset.data.shift();
          continue;
        }

        isPastLimit = true;
      } while (!isPastLimit);
    }

    // Trigger chart update after purging data
    this.baseChart.update();
  }
}
