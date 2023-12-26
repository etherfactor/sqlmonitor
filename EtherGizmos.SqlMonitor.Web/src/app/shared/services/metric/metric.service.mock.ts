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
        id: parseGuid('2f4ec7b4-4e81-4ac5-8030-7d7399dc2097'),
        name: 'CPU Utilization',
        aggregateType: AggregateType.Average,
        severities: [],
      },
      {
        id: parseGuid('8b140817-ec33-4dc7-9c4f-4bf6d8098b3c'),
        name: 'Memory Utilization',
        aggregateType: AggregateType.Average,
        severities: [],
      },
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
