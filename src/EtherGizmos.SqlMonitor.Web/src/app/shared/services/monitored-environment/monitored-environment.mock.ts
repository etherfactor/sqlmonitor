import { Injectable, Provider } from "@angular/core";
import { DateTime } from "luxon";
import { Observable, delay, of, throwError } from "rxjs";
import { MonitoredEnvironment } from "../../models/monitored-environment";
import { Guid, generateGuid } from "../../types/guid/guid";
import { ɵEntitySet } from "../../utilities/odata/internal/entity-set";
import { EntitySet } from "../../utilities/odata/odata.util";
import { MonitoredEnvironmentService } from "./monitored-environment.service";

const cache: { [key: Guid]: MonitoredEnvironment } = {};
cache['715b1882-ad9c-4469-b627-f16e3512ac57' as Guid] = {
  id: '715b1882-ad9c-4469-b627-f16e3512ac57' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: undefined,
  modifiedByUserId: undefined,
  name: 'Example Environment',
  description: 'I am an example environment.',
  isActive: true,
};

@Injectable({
  providedIn: 'root'
})
class MockMonitoredEnvironmentService extends MonitoredEnvironmentService {

  override get(id: Guid): Observable<MonitoredEnvironment> {
    const maybeRecord = cache[id];

    if (!maybeRecord) {
      return throwError(() => new Error('Record does not exist'));
    }

    return of({ ...maybeRecord }).pipe(
      delay(1000)
    );
  }

  override get set(): EntitySet<MonitoredEnvironment> {
    return new ɵEntitySet.MockImplementation<MonitoredEnvironment>(() => {
      return Object.keys(cache).map(key => cache[key as Guid]);
    });
  }

  override search(): Observable<MonitoredEnvironment[]> {
    throw new Error("Method not implemented.");
  }

  override create(record: Partial<MonitoredEnvironment>): Observable<MonitoredEnvironment> {
    record = { ...record };
    record.id = generateGuid();
    record.createdAt = DateTime.now();
    record.createdByUserId = generateGuid();

    cache[record.id] = record as MonitoredEnvironment;

    return of({ ...record } as MonitoredEnvironment).pipe(
      delay(1000)
    );
  }

  override update(id: Guid, record: Partial<MonitoredEnvironment>): Observable<MonitoredEnvironment> {
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

  override delete(id: Guid): Observable<void> {
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

export function provideMonitoredEnvironmentServiceMock(): Provider {
  return {
    provide: MonitoredEnvironmentService,
    useClass: MockMonitoredEnvironmentService,
  };
}
