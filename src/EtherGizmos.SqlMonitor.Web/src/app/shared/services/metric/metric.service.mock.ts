import { Injectable, Provider } from "@angular/core";
import { DateTime } from "luxon";
import { Observable, delay, of, throwError } from "rxjs";
import { AggregateType } from "../../models/aggregate-type";
import { Metric } from "../../models/metric";
import { Guid, generateGuid } from "../../types/guid/guid";
import { ɵEntitySet } from "../../utilities/odata/internal/entity-set";
import { EntitySet } from "../../utilities/odata/odata.util";
import { MetricService } from "./metric.service";

let increment = 0;

const cache: { [key: number]: Metric } = {};
cache[++increment] = {
  id: increment,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: undefined,
  modifiedByUserId: undefined,
  name: 'Example Metric',
  description: 'I am an example metric.',
  aggregateType: AggregateType.Average,
  isActive: true,
};

cache[++increment] = {
  id: increment,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: DateTime.now(),
  modifiedByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  name: 'CPU Usage',
  description: 'Tracks CPU usage over time.',
  aggregateType: AggregateType.Maximum,
  isActive: true,
};

cache[++increment] = {
  id: increment,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: DateTime.now(),
  modifiedByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  name: 'Memory Usage',
  description: 'Monitors memory usage and peak usage.',
  aggregateType: AggregateType.Maximum,
  isActive: true,
};

cache[++increment] = {
  id: increment,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: undefined,
  modifiedByUserId: undefined,
  name: 'Disk I/O',
  description: 'Records disk input/output operations.',
  aggregateType: AggregateType.Sum,
  isActive: false,
};

cache[++increment] = {
  id: increment,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: DateTime.now(),
  modifiedByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  name: 'Network Traffic',
  description: 'Measures incoming and outgoing network traffic.',
  aggregateType: AggregateType.Average,
  isActive: true,
};

@Injectable({
  providedIn: 'root'
})
class MockMetricService extends MetricService {

  override get(id: number): Observable<Metric> {
    const maybeRecord = cache[id];

    if (!maybeRecord) {
      return throwError(() => new Error('Record does not exist'));
    }

    return of({ ...maybeRecord }).pipe(
      delay(1000)
    );
  }

  override get set(): EntitySet<Metric> {
    return new ɵEntitySet.MockImplementation<Metric>(() => {
      return Object.keys(cache).map(key => cache[key as unknown as number]);
    });
  }

  override search(): Observable<Metric[]> {
    throw new Error("Method not implemented.");
  }

  override create(record: Partial<Metric>): Observable<Metric> {
    record = { ...record };
    record.id = ++increment;
    record.createdAt = DateTime.now();
    record.createdByUserId = generateGuid();

    cache[record.id] = record as Metric;

    return of({ ...record } as Metric).pipe(
      delay(1000)
    );
  }

  override update(id: number, record: Partial<Metric>): Observable<Metric> {
    const maybeRecord = cache[id];

    if (!maybeRecord) {
      return throwError(() => new Error('Record does not exist'));
    }

    record = { ...record };
    Object.assign(maybeRecord, record);
    cache[id] = maybeRecord;

    return of({ ...maybeRecord }).pipe(
      delay(1000)
    );
  }

  override delete(id: number): Observable<void> {
    const maybeRecord = cache[id];

    if (!maybeRecord) {
      return throwError(() => new Error('Record does not exist'));
    }

    delete cache[id];

    return of(void 0).pipe(
      delay(1000)
    );
  }
}

export function provideMetricServiceMock(): Provider {
  return {
    provide: MetricService,
    useClass: MockMetricService,
  };
}
