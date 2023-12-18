import { Injectable, Provider } from "@angular/core";
import { MetricService } from "./metric.service";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { Metric } from "../../models/metric";

@Injectable({
  providedIn: 'root'
})
class ConcreteMetricService extends MetricService {

  private $http: HttpClient;

  constructor(
    $http: HttpClient,
  ) {
    super();
    this.$http = $http;
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
