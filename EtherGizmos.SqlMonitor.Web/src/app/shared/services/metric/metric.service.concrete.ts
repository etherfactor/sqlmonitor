import { HttpClient } from "@angular/common/http";
import { Provider } from "@angular/core";
import { Observable } from "rxjs";
import { Metric } from "../../models/metric";
import { Guid } from "../../types/guid/guid";
import { MetricService } from "./metric.service";

class ConcreteMetricService extends MetricService {

  private $http: HttpClient;

  constructor(
    $http: HttpClient,
  ) {
    super();
    this.$http = $http;
  }

  override get(id: Guid): Observable<Metric> {
    throw new Error("Method not implemented.");
  }

  override search(): Observable<Metric[]> {
    throw new Error("Method not implemented.");
  }
}

export function provideMetricService(): Provider {
  return {
    provide: MetricService,
    useFactory: (
      $http: HttpClient,
    ) => new ConcreteMetricService(
      $http,
    )
  };
}
