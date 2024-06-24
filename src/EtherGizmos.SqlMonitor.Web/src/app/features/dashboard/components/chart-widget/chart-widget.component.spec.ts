import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Point } from 'chart.js';
import { DateTime } from 'luxon';
import { AggregateType } from '../../../../shared/models/aggregate-type';
import { SeverityType } from '../../../../shared/models/severity-type';
import { provideInstanceServiceMock } from '../../../../shared/services/instance/instance.service.mock';
import { MetricDataService } from '../../../../shared/services/metric-data/metric-data.service';
import { provideMetricDataServiceMock } from '../../../../shared/services/metric-data/metric-data.service.mock';
import { provideMetricServiceMock } from '../../../../shared/services/metric/metric.service.mock';
import { generateGuid, parseGuid } from '../../../../shared/types/guid/guid';
import { DashboardWidget, DashboardWidgetChartMetricBucketType, DashboardWidgetChartScaleType, DashboardWidgetType } from '../../models/dashboard-widget';
import { AverageMetricAggregator, ChartWidgetComponent, MaximumMetricAggregator, MinimumMetricAggregator, StandardDeviationMetricAggregator, SumMetricAggregator, VarianceMetricAggregator } from './chart-widget.component';

describe('ChartWidgetComponent', () => {
  let component: ChartWidgetComponent;
  let fixture: ComponentFixture<ChartWidgetComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        ChartWidgetComponent,
      ],
      providers: [
        provideInstanceServiceMock(),
        provideMetricServiceMock(),
        provideMetricDataServiceMock(),
      ]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ChartWidgetComponent);
    component = fixture.componentInstance;
    component.config = {
      id: generateGuid(),
      type: DashboardWidgetType.Chart,
      grid: {
        xPos: 0,
        yPos: 0,
        width: 1,
        height: 1,
        hovering: false,
      },
      chart: {
        colors: [],
        xScale: {
          id: 'X',
          type: DashboardWidgetChartScaleType.Time,
          minEnforced: false,
          maxEnforced: false,
          stacked: false,
        },
        yScales: [],
        metrics: [],
      },
    };
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should update config on init', () => {
    const updateSpy = spyOn(component as any, 'updateWidget');
    expect(updateSpy).not.toHaveBeenCalled();

    component.ngOnInit();
    expect(updateSpy).toHaveBeenCalled();
  });

  it('should flush subscriptions on update', () => {
    const flushSubscriptionsSpy = spyOn(component, 'flushSubscriptions');
    expect(flushSubscriptionsSpy).not.toHaveBeenCalled();

    (component as any).updateWidget(component.config);
    expect(flushSubscriptionsSpy).toHaveBeenCalled();
  });

  it('should subscribe to metrics on update', () => {
    const $metricData = TestBed.inject(MetricDataService);
    const subscribeSpy = spyOn($metricData, 'subscribe').and.callThrough();

    const newConfig: DashboardWidget = {
      id: generateGuid(),
      type: DashboardWidgetType.Chart,
      grid: {
        xPos: 0,
        yPos: 0,
        width: 1,
        height: 1,
        hovering: false,
      },
      chart: {
        colors: [],
        xScale: {
          id: 'X',
          type: DashboardWidgetChartScaleType.Time,
          minEnforced: false,
          maxEnforced: false,
          stacked: false,
        },
        yScales: [
          {
            id: 'Y',
            type: DashboardWidgetChartScaleType.Linear,
            minEnforced: false,
            maxEnforced: false,
            stacked: false,
          },
        ],
        metrics: [
          {
            id: generateGuid(),
            yScaleId: 'Y',
            metricId: parseGuid('2f4ec7b4-4e81-4ac5-8030-7d7399dc2097'),
            bucketType: DashboardWidgetChartMetricBucketType.SpecificBuckets,
            bucketAggregateType: AggregateType.Average,
            buckets: ['A'],
          },
          {
            id: generateGuid(),
            yScaleId: 'Y',
            metricId: parseGuid('8b140817-ec33-4dc7-9c4f-4bf6d8098b3c'),
            bucketType: DashboardWidgetChartMetricBucketType.SpecificBuckets,
            bucketAggregateType: AggregateType.Average,
            buckets: ['B'],
          },
        ],
      },
    };

    (component as any).updateWidget(newConfig);

    expect(subscribeSpy).toHaveBeenCalledTimes(2);
  });
});

describe('AverageMetricAggregator', () => {
  it('should calculate the average', () => {
    const now = DateTime.now();

    const aggregator = new AverageMetricAggregator([]);

    expect(aggregator.data.length).toBe(0);

    aggregator.addDatum({
      metricId: parseGuid('2f4ec7b4-4e81-4ac5-8030-7d7399dc2097'),
      instanceId: parseGuid('006db56a-d3b1-4c50-af8f-f19bb13f85ed'),
      eventTimeUtc: now,
      severityType: SeverityType.Nominal,
      value: 0,
    });
    expect(aggregator.data.length).toBe(1);
    expect((aggregator.data[0] as Point).y).toBe(0);

    aggregator.addDatum({
      metricId: parseGuid('2f4ec7b4-4e81-4ac5-8030-7d7399dc2097'),
      instanceId: parseGuid('006db56a-d3b1-4c50-af8f-f19bb13f85ed'),
      eventTimeUtc: now,
      severityType: SeverityType.Nominal,
      value: 1,
    });
    expect(aggregator.data.length).toBe(1);
    expect((aggregator.data[0] as Point).y).toBe(0.5);

    aggregator.addDatum({
      metricId: parseGuid('2f4ec7b4-4e81-4ac5-8030-7d7399dc2097'),
      instanceId: parseGuid('006db56a-d3b1-4c50-af8f-f19bb13f85ed'),
      eventTimeUtc: now,
      severityType: SeverityType.Nominal,
      value: 0,
    });
    aggregator.addDatum({
      metricId: parseGuid('2f4ec7b4-4e81-4ac5-8030-7d7399dc2097'),
      instanceId: parseGuid('006db56a-d3b1-4c50-af8f-f19bb13f85ed'),
      eventTimeUtc: now,
      severityType: SeverityType.Nominal,
      value: 0,
    });
    expect(aggregator.data.length).toBe(1);
    expect((aggregator.data[0] as Point).y).toBe(0.25);
  });
});

describe('MaximumMetricAggregator', () => {
  it('should calculate the maximum', () => {
    const now = DateTime.now();

    const aggregator = new MaximumMetricAggregator([]);

    expect(aggregator.data.length).toBe(0);

    aggregator.addDatum({
      metricId: parseGuid('2f4ec7b4-4e81-4ac5-8030-7d7399dc2097'),
      instanceId: parseGuid('006db56a-d3b1-4c50-af8f-f19bb13f85ed'),
      eventTimeUtc: now,
      severityType: SeverityType.Nominal,
      value: 0,
    });
    expect(aggregator.data.length).toBe(1);
    expect((aggregator.data[0] as Point).y).toBe(0);

    aggregator.addDatum({
      metricId: parseGuid('2f4ec7b4-4e81-4ac5-8030-7d7399dc2097'),
      instanceId: parseGuid('006db56a-d3b1-4c50-af8f-f19bb13f85ed'),
      eventTimeUtc: now,
      severityType: SeverityType.Nominal,
      value: 1,
    });
    expect(aggregator.data.length).toBe(1);
    expect((aggregator.data[0] as Point).y).toBe(1);

    aggregator.addDatum({
      metricId: parseGuid('2f4ec7b4-4e81-4ac5-8030-7d7399dc2097'),
      instanceId: parseGuid('006db56a-d3b1-4c50-af8f-f19bb13f85ed'),
      eventTimeUtc: now,
      severityType: SeverityType.Nominal,
      value: 0,
    });
    aggregator.addDatum({
      metricId: parseGuid('2f4ec7b4-4e81-4ac5-8030-7d7399dc2097'),
      instanceId: parseGuid('006db56a-d3b1-4c50-af8f-f19bb13f85ed'),
      eventTimeUtc: now,
      severityType: SeverityType.Nominal,
      value: 0,
    });
    expect(aggregator.data.length).toBe(1);
    expect((aggregator.data[0] as Point).y).toBe(1);
  });
});

describe('MinimumMetricAggregator', () => {
  it('should calculate the minimum', () => {
    const now = DateTime.now();

    const aggregator = new MinimumMetricAggregator([]);

    expect(aggregator.data.length).toBe(0);

    aggregator.addDatum({
      metricId: parseGuid('2f4ec7b4-4e81-4ac5-8030-7d7399dc2097'),
      instanceId: parseGuid('006db56a-d3b1-4c50-af8f-f19bb13f85ed'),
      eventTimeUtc: now,
      severityType: SeverityType.Nominal,
      value: 1,
    });
    expect(aggregator.data.length).toBe(1);
    expect((aggregator.data[0] as Point).y).toBe(1);

    aggregator.addDatum({
      metricId: parseGuid('2f4ec7b4-4e81-4ac5-8030-7d7399dc2097'),
      instanceId: parseGuid('006db56a-d3b1-4c50-af8f-f19bb13f85ed'),
      eventTimeUtc: now,
      severityType: SeverityType.Nominal,
      value: 0,
    });
    expect(aggregator.data.length).toBe(1);
    expect((aggregator.data[0] as Point).y).toBe(0);

    aggregator.addDatum({
      metricId: parseGuid('2f4ec7b4-4e81-4ac5-8030-7d7399dc2097'),
      instanceId: parseGuid('006db56a-d3b1-4c50-af8f-f19bb13f85ed'),
      eventTimeUtc: now,
      severityType: SeverityType.Nominal,
      value: 1,
    });
    aggregator.addDatum({
      metricId: parseGuid('2f4ec7b4-4e81-4ac5-8030-7d7399dc2097'),
      instanceId: parseGuid('006db56a-d3b1-4c50-af8f-f19bb13f85ed'),
      eventTimeUtc: now,
      severityType: SeverityType.Nominal,
      value: 1,
    });
    expect(aggregator.data.length).toBe(1);
    expect((aggregator.data[0] as Point).y).toBe(0);
  });
});

describe('StandardDeviationMetricAggregator', () => {
  it('should calculate the standard deviation', () => {
    const now = DateTime.now();

    const aggregator = new StandardDeviationMetricAggregator([]);

    expect(aggregator.data.length).toBe(0);

    aggregator.addDatum({
      metricId: parseGuid('2f4ec7b4-4e81-4ac5-8030-7d7399dc2097'),
      instanceId: parseGuid('006db56a-d3b1-4c50-af8f-f19bb13f85ed'),
      eventTimeUtc: now,
      severityType: SeverityType.Nominal,
      value: 1,
    });
    expect(aggregator.data.length).toBe(1);
    expect((aggregator.data[0] as Point).y).toBe(0);

    aggregator.addDatum({
      metricId: parseGuid('2f4ec7b4-4e81-4ac5-8030-7d7399dc2097'),
      instanceId: parseGuid('006db56a-d3b1-4c50-af8f-f19bb13f85ed'),
      eventTimeUtc: now,
      severityType: SeverityType.Nominal,
      value: 0,
    });
    expect(aggregator.data.length).toBe(1);
    expect((aggregator.data[0] as Point).y).toBe(0.5);

    aggregator.addDatum({
      metricId: parseGuid('2f4ec7b4-4e81-4ac5-8030-7d7399dc2097'),
      instanceId: parseGuid('006db56a-d3b1-4c50-af8f-f19bb13f85ed'),
      eventTimeUtc: now,
      severityType: SeverityType.Nominal,
      value: 1,
    });
    aggregator.addDatum({
      metricId: parseGuid('2f4ec7b4-4e81-4ac5-8030-7d7399dc2097'),
      instanceId: parseGuid('006db56a-d3b1-4c50-af8f-f19bb13f85ed'),
      eventTimeUtc: now,
      severityType: SeverityType.Nominal,
      value: 1,
    });
    expect(aggregator.data.length).toBe(1);
    expect((aggregator.data[0] as Point).y).toBeCloseTo(0.4330, 0.001);
  });
});

describe('SumMetricAggregator', () => {
  it('should calculate the sum', () => {
    const now = DateTime.now();

    const aggregator = new SumMetricAggregator([]);

    expect(aggregator.data.length).toBe(0);

    aggregator.addDatum({
      metricId: parseGuid('2f4ec7b4-4e81-4ac5-8030-7d7399dc2097'),
      instanceId: parseGuid('006db56a-d3b1-4c50-af8f-f19bb13f85ed'),
      eventTimeUtc: now,
      severityType: SeverityType.Nominal,
      value: 1,
    });
    expect(aggregator.data.length).toBe(1);
    expect((aggregator.data[0] as Point).y).toBe(1);

    aggregator.addDatum({
      metricId: parseGuid('2f4ec7b4-4e81-4ac5-8030-7d7399dc2097'),
      instanceId: parseGuid('006db56a-d3b1-4c50-af8f-f19bb13f85ed'),
      eventTimeUtc: now,
      severityType: SeverityType.Nominal,
      value: 0,
    });
    expect(aggregator.data.length).toBe(1);
    expect((aggregator.data[0] as Point).y).toBe(1);

    aggregator.addDatum({
      metricId: parseGuid('2f4ec7b4-4e81-4ac5-8030-7d7399dc2097'),
      instanceId: parseGuid('006db56a-d3b1-4c50-af8f-f19bb13f85ed'),
      eventTimeUtc: now,
      severityType: SeverityType.Nominal,
      value: 1,
    });
    aggregator.addDatum({
      metricId: parseGuid('2f4ec7b4-4e81-4ac5-8030-7d7399dc2097'),
      instanceId: parseGuid('006db56a-d3b1-4c50-af8f-f19bb13f85ed'),
      eventTimeUtc: now,
      severityType: SeverityType.Nominal,
      value: 1,
    });
    expect(aggregator.data.length).toBe(1);
    expect((aggregator.data[0] as Point).y).toBe(3);
  });
});

describe('VarianceMetricAggregator', () => {
  it('should calculate the variance', () => {
    const now = DateTime.now();

    const aggregator = new VarianceMetricAggregator([]);

    expect(aggregator.data.length).toBe(0);

    aggregator.addDatum({
      metricId: parseGuid('2f4ec7b4-4e81-4ac5-8030-7d7399dc2097'),
      instanceId: parseGuid('006db56a-d3b1-4c50-af8f-f19bb13f85ed'),
      eventTimeUtc: now,
      severityType: SeverityType.Nominal,
      value: 1,
    });
    expect(aggregator.data.length).toBe(1);
    expect((aggregator.data[0] as Point).y).toBe(0);

    aggregator.addDatum({
      metricId: parseGuid('2f4ec7b4-4e81-4ac5-8030-7d7399dc2097'),
      instanceId: parseGuid('006db56a-d3b1-4c50-af8f-f19bb13f85ed'),
      eventTimeUtc: now,
      severityType: SeverityType.Nominal,
      value: 0,
    });
    expect(aggregator.data.length).toBe(1);
    expect((aggregator.data[0] as Point).y).toBe(0.25);

    aggregator.addDatum({
      metricId: parseGuid('2f4ec7b4-4e81-4ac5-8030-7d7399dc2097'),
      instanceId: parseGuid('006db56a-d3b1-4c50-af8f-f19bb13f85ed'),
      eventTimeUtc: now,
      severityType: SeverityType.Nominal,
      value: 1,
    });
    aggregator.addDatum({
      metricId: parseGuid('2f4ec7b4-4e81-4ac5-8030-7d7399dc2097'),
      instanceId: parseGuid('006db56a-d3b1-4c50-af8f-f19bb13f85ed'),
      eventTimeUtc: now,
      severityType: SeverityType.Nominal,
      value: 1,
    });
    expect(aggregator.data.length).toBe(1);
    expect((aggregator.data[0] as Point).y).toBeCloseTo(0.1875, 0.001);
  });
});
