import { HttpClient } from "@angular/common/http";
import { Injectable, Provider } from "@angular/core";
import { MetricDataService } from "./metric-data.service";
import { MetricDataCacheService } from "../metric-data-cache/metric-data-cache.service";

@Injectable({ providedIn: 'root' })
class ConcreteMetricDataService extends MetricDataService {

  private $http: HttpClient;

  constructor(
    $http: HttpClient,
    $metricDataCache: MetricDataCacheService,
  ) {
    super($metricDataCache);
    this.$http = $http;
  }
}

export function provideMetricDataService(): Provider {
  return {
    provide: MetricDataService,
    useFactory: (
      $http: HttpClient,
      $metricDataCache: MetricDataCacheService,
    ) => new ConcreteMetricDataService(
      $http,
      $metricDataCache,
    )
  };
}
