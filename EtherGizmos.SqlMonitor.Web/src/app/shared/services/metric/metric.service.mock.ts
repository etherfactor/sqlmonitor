import { Provider } from "@angular/core";
import { Observable, of } from "rxjs";
import { v4 as uuidv4 } from 'uuid';
import { AggregateType } from "../../models/aggregate-type";
import { Metric } from "../../models/metric";
import { parseGuid } from "../../types/guid/guid";
import { MetricService } from "./metric.service";

class MockMetricService extends MetricService {

  metrics: Metric[];

  constructor() {
    super();

    this.metrics = [
      {
        id: parseGuid(uuidv4()),
        name: 'Memory Utilization',
        aggregateType: AggregateType.Average,
        severities: [],
      }
    ];
  }

  override search(): Observable<Metric[]> {
    return of(this.metrics);
  }
}

export function provideMetricServiceMock(): Provider {
  return {
    provide: MetricService,
    useFactory: () => new MockMetricService(),
  };
}
