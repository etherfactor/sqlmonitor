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

cache['adf4138c-e091-4d73-8f8f-dc8ac9e5e7e9' as Guid] = {
  id: 'adf4138c-e091-4d73-8f8f-dc8ac9e5e7e9' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: DateTime.now(),
  modifiedByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  name: 'Production Server',
  description: 'Handles production workload.',
  isActive: true,
};

cache['7c4d0f58-67d5-4b7c-b1d8-bb5410efdfb1' as Guid] = {
  id: '7c4d0f58-67d5-4b7c-b1d8-bb5410efdfb1' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: DateTime.now(),
  modifiedByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  name: 'Test Environment',
  description: 'Used for testing new features.',
  isActive: false,
};

cache['c7e9b9a3-8f6f-4e79-b98c-9f9ef2926b49' as Guid] = {
  id: 'c7e9b9a3-8f6f-4e79-b98c-9f9ef2926b49' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: undefined,
  modifiedByUserId: undefined,
  name: 'Development Server',
  description: 'For development and local testing.',
  isActive: true,
};

cache['1b2c9e29-1a6b-4182-8eaf-7c7692f0302e' as Guid] = {
  id: '1b2c9e29-1a6b-4182-8eaf-7c7692f0302e' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: DateTime.now(),
  modifiedByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  name: 'Staging Server',
  description: 'Mimics production environment for final testing.',
  isActive: false,
};

cache['8b6f75b3-0f09-45e4-9e5c-b1585463c9ef' as Guid] = {
  id: '8b6f75b3-0f09-45e4-9e5c-b1585463c9ef' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: undefined,
  modifiedByUserId: undefined,
  name: 'Backup Server',
  description: 'Used for backup and recovery processes.',
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
    return new ɵEntitySet.MockImplementation<MonitoredSystem>(() => {
      return Object.keys(cache).map(key => cache[key as Guid]);
    });
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
