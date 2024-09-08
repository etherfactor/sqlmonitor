import { Injectable, Provider } from "@angular/core";
import { DateTime } from "luxon";
import { Observable, delay, of, throwError } from "rxjs";
import { MonitoredResource } from "../../models/monitored-resource";
import { Guid, generateGuid } from "../../types/guid/guid";
import { ɵEntitySet } from "../../utilities/odata/internal/entity-set";
import { EntitySet } from "../../utilities/odata/odata.util";
import { MonitoredResourceService } from "./monitored-resource.service";

const cache: { [key: Guid]: MonitoredResource } = {};
cache['334adfee-557f-4f40-a909-41877fadfb09' as Guid] = {
  id: '334adfee-557f-4f40-a909-41877fadfb09' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: undefined,
  modifiedByUserId: undefined,
  name: 'Example Resource',
  description: 'I am an example resource.',
  isActive: true,
};

cache['b9c8d8ab-2745-4bc2-8591-9be9f5f27034' as Guid] = {
  id: 'b9c8d8ab-2745-4bc2-8591-9be9f5f27034' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: DateTime.now(),
  modifiedByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  name: 'Database Instance',
  description: 'Primary database for storing user data.',
  isActive: true,
};

cache['8c1f26a1-d2c1-419c-9468-6e95b9cc7d72' as Guid] = {
  id: '8c1f26a1-d2c1-419c-9468-6e95b9cc7d72' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: DateTime.now(),
  modifiedByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  name: 'File Storage',
  description: 'Storage for user-uploaded files.',
  isActive: false,
};

cache['d71f5e2a-2ef7-4a23-a60c-23510a2e3c50' as Guid] = {
  id: 'd71f5e2a-2ef7-4a23-a60c-23510a2e3c50' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: undefined,
  modifiedByUserId: undefined,
  name: 'API Gateway',
  description: 'Handles requests and routes them to the appropriate services.',
  isActive: true,
};

cache['b2e78836-ec5c-4fd5-8a63-49a622c2e8d2' as Guid] = {
  id: 'b2e78836-ec5c-4fd5-8a63-49a622c2e8d2' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: DateTime.now(),
  modifiedByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  name: 'Cache Service',
  description: 'In-memory caching for faster access to frequently used data.',
  isActive: false,
};

cache['71e7b5c4-2eb5-4187-9b8a-c2de64c9e40e' as Guid] = {
  id: '71e7b5c4-2eb5-4187-9b8a-c2de64c9e40e' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: undefined,
  modifiedByUserId: undefined,
  name: 'Monitoring Agent',
  description: 'Tracks system performance and sends alerts.',
  isActive: true,
};

@Injectable({
  providedIn: 'root'
})
class MockMonitoredResourceService extends MonitoredResourceService {

  override get(id: Guid): Observable<MonitoredResource> {
    const maybeRecord = cache[id];

    if (!maybeRecord) {
      return throwError(() => new Error('Record does not exist'));
    }

    return of({ ...maybeRecord }).pipe(
      delay(1000)
    );
  }

  override get set(): EntitySet<MonitoredResource> {
    return new ɵEntitySet.MockImplementation<MonitoredResource>(() => {
      return Object.keys(cache).map(key => cache[key as Guid]);
    });
  }

  override search(): Observable<MonitoredResource[]> {
    throw new Error("Method not implemented.");
  }

  override create(record: Partial<MonitoredResource>): Observable<MonitoredResource> {
    record = { ...record };
    record.id = generateGuid();
    record.createdAt = DateTime.now();
    record.createdByUserId = generateGuid();

    cache[record.id] = record as MonitoredResource;

    return of({ ...record } as MonitoredResource).pipe(
      delay(1000)
    );
  }

  override update(id: Guid, record: Partial<MonitoredResource>): Observable<MonitoredResource> {
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

export function provideMonitoredResourceServiceMock(): Provider {
  return {
    provide: MonitoredResourceService,
    useClass: MockMonitoredResourceService,
  };
}
