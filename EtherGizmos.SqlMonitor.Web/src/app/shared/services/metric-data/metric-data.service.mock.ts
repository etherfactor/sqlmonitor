import { Injectable, Provider } from "@angular/core";
import { DateTime } from "luxon";
import { Observable, delay, of, repeat, switchMap } from "rxjs";
import { MetricData } from "../../models/metric-data";
import { SeverityType } from "../../models/severity-type";
import { Guid, parseGuid } from "../../types/guid/guid";
import { MetricDataService } from "./metric-data.service";

@Injectable({ providedIn: 'root' })
class MockMetricDataService extends MetricDataService {

  constructor() {
    super();
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
    useFactory: () => new MockMetricDataService(),
  };
}
