import { Injectable, Provider } from "@angular/core";
import { DateTime } from "luxon";
import { Observable, delay, of, throwError } from "rxjs";
import { MonitoredSystem } from "../../models/monitored-system";
import { Guid, generateGuid } from "../../types/guid/guid";
import { ɵEntitySet } from "../../utilities/odata/internal/entity-set";
import { EntitySet } from "../../utilities/odata/odata.util";
import { MonitoredSystemService } from "./monitored-system.service";

const cache: { [key: Guid]: MonitoredSystem } = {};
cache['fca5315f-6e2f-4a78-baac-bdb061e6d8fc' as Guid] = {
  id: 'fca5315f-6e2f-4a78-baac-bdb061e6d8fc' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: undefined,
  modifiedByUserId: undefined,
  name: 'Example System',
  description: 'I am an example system.',
  isActive: true,
};

@Injectable({
  providedIn: 'root'
})
class MockMonitoredSystemService extends MonitoredSystemService {

  override get(id: Guid): Observable<MonitoredSystem> {
    const maybeRecord = cache[id];

    if (!maybeRecord) {
      return throwError(() => new Error('Record does not exist'));
    }

    return of({ ...maybeRecord }).pipe(
      delay(1000)
    );
  }

  override get set(): EntitySet<MonitoredSystem> {
    return new ɵEntitySet.Implementation<MonitoredSystem>();
  }

  override search(): Observable<MonitoredSystem[]> {
    throw new Error("Method not implemented.");
  }

  override create(record: Partial<MonitoredSystem>): Observable<MonitoredSystem> {
    record = { ...record };
    record.id = generateGuid();
    record.createdAt = DateTime.now();
    record.createdByUserId = generateGuid();

    cache[record.id] = record as MonitoredSystem;

    return of({ ...record } as MonitoredSystem).pipe(
      delay(1000)
    );
  }

  override update(id: Guid, record: Partial<MonitoredSystem>): Observable<MonitoredSystem> {
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

export function provideMonitoredSystemServiceMock(): Provider {
  return {
    provide: MonitoredSystemService,
    useClass: MockMonitoredSystemService,
  };
}
