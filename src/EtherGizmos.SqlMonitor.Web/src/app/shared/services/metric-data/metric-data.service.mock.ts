import { Injectable, Provider } from "@angular/core";
import { MetricDataCacheService } from "../metric-data-cache/metric-data-cache.service";
import { MetricDataService } from "./metric-data.service";

@Injectable({ providedIn: 'root' })
class MockMetricDataService extends MetricDataService {

  constructor(
    $metricDataCache: MetricDataCacheService,
  ) {
    super($metricDataCache);
  }
}

export function provideMetricDataServiceMock(): Provider {
  return {
    provide: MetricDataService,
    useFactory: ($metricDataCache: MetricDataCacheService) => new MockMetricDataService($metricDataCache),
    deps: [MetricDataCacheService],
  };
}
