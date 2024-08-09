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
