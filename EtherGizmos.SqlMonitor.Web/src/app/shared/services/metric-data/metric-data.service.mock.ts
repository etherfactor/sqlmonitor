import { Injectable, Provider } from "@angular/core";
import { MetricDataService } from "./metric-data.service";

@Injectable({ providedIn: 'root' })
class MockMetricDataService extends MetricDataService {

  constructor() {
    super();
  }
}

export function provideMetricDataServiceMock(): Provider {
  return {
    provide: MetricDataService,
    useFactory: () => new MockMetricDataService(),
  };
}
