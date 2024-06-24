import { Injectable, Provider } from '@angular/core';
import { Observable, of, throwError } from 'rxjs';
import { Dashboard } from '../../../features/dashboard/models/dashboard';
import { DashboardWidgetChartMetricBucketType, DashboardWidgetChartScaleType, DashboardWidgetChartType, DashboardWidgetType } from '../../../features/dashboard/models/dashboard-widget';
import { AggregateType } from '../../models/aggregate-type';
import { Guid, parseGuid } from '../../types/guid/guid';
import { parseRelativeTime } from '../../types/relative-time/relative-time';
import { DashboardService } from './dashboard.service';

@Injectable({
  providedIn: 'root'
})
class MockDashboardService extends DashboardService {

  dashboards: Dashboard[];

  constructor() {
    super();

    this.dashboards = [
      {
        id: parseGuid('05e38a95-0d4c-4527-a6a6-6b9f2c0ea0ff'),
        name: 'Test Dashboard',
        description: 'Contains a simple chart.',
        timeStart: parseRelativeTime("t-5m"),
        timeEnd: parseRelativeTime("t"),
        instanceIds: [
          parseGuid("006db56a-d3b1-4c50-af8f-f19bb13f85ed"),
        ],
        items: [
          {
            id: parseGuid("61c200c4-f449-4a1a-89e4-cf1eaff8aee3"),
            type: DashboardWidgetType.Chart,
            grid: {
              xPos: 4,
              yPos: 0,
              width: 4,
              height: 5,
              hovering: false,
            },
            chart: {
              type: DashboardWidgetChartType.Line,
              colors: [
                "#ffffff",
                "#ffffff",
                "#ffffff",
                "#ffffff",
                "#ffffff",
                "#ffffff",
                "#ffffff",
                "#ffffff",
              ],
              xScale: {
                id: "X",
                type: DashboardWidgetChartScaleType.Time,
                label: undefined,
                min: undefined,
                minEnforced: false,
                max: undefined,
                maxEnforced: false,
                stacked: false,
              },
              yScales: [
                {
                  id: "Y",
                  type: DashboardWidgetChartScaleType.Linear,
                  label: undefined,
                  min: 0,
                  minEnforced: false,
                  max: 1,
                  maxEnforced: false,
                  stacked: false,
                }
              ],
              metrics: [
                {
                  id: parseGuid("0e688e1f-bc8c-4eb1-9276-aba80ad1fbd3"),
                  metricId: parseGuid( "2f4ec7b4-4e81-4ac5-8030-7d7399dc2097"),
                  yScaleId: "Y",
                  label: "CPU",
                  bucketType: DashboardWidgetChartMetricBucketType.Aggregate,
                  bucketAggregateType: AggregateType.Unknown,
                  buckets: [],
                  bucketTopN: undefined,
                }
              ]
            },
          }
        ]
      },
    ];
  }

  override get(id: Guid): Observable<Dashboard> {
    const index = this.dashboards.findIndex(e => e.id === id);
    if (index >= 0) {
      return of(this.dashboards[index]);
    }

    return throwError(() => new Error('Dashboard not found.'));
  }

  override search(): Observable<Dashboard[]> {
    return of(this.dashboards);
  }
}

export function provideDashboardServiceMock(): Provider {
  return {
    provide: DashboardService,
    useFactory: () => new MockDashboardService(),
  };
}
