import { HttpClient } from "@angular/common/http";
import { Injectable, Provider } from "@angular/core";
import { MetricDataService } from "./metric-data.service";

@Injectable({ providedIn: 'root' })
class ConcreteMetricDataService extends MetricDataService {

  private $http: HttpClient;

  constructor(
    $http: HttpClient,
  ) {
    super();
    this.$http = $http;
  }
}

export function provideMetricDataService(): Provider {
  return {
    provide: MetricDataService,
    useFactory: (
      $http: HttpClient,
    ) => new ConcreteMetricDataService(
      $http,
    )
  };
}
