import { Injectable, Provider } from "@angular/core";
import { DateTime } from "luxon";
import { Observable, delay, of, repeat, switchMap } from "rxjs";
import { MetricData } from "../../models/metric-data";
import { SeverityType } from "../../models/severity-type";
import { Guid, parseGuid } from "../../types/guid/guid";
import { MetricDataCacheService } from "../metric-data-cache/metric-data-cache.service";
import { MetricDataService } from "./metric-data.service";

@Injectable({ providedIn: 'root' })
class MockMetricDataService extends MetricDataService {

  constructor(
    $metricDataCache: MetricDataCacheService,
  ) {
    super($metricDataCache);
  }

  watchMetricData(id: Guid): Observable<MetricData> {
    return of('').pipe(
      switchMap(() => {
        const data: MetricData = {
          instanceId: parseGuid('00000000-0000-0000-0000-000000000000'),
          metricId: id,
          eventTimeUtc: DateTime.now(),
          bucket: undefined,
          severityType: SeverityType.Nominal,
          value: Math.random(),
        };
        return of(data);
      }),
      delay(1000),
      repeat(),
    );
  }
}

export function provideMetricDataServiceMock(): Provider {
  return {
    provide: MetricDataService,
    useFactory: ($metricDataCache: MetricDataCacheService) => new MockMetricDataService($metricDataCache),
    deps: [MetricDataCacheService],
  };
}
