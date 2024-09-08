import { Injectable, Provider } from "@angular/core";
import { DateTime } from "luxon";
import { Observable, delay, of, throwError } from "rxjs";
import { Group } from "../../models/group";
import { Guid, generateGuid } from "../../types/guid/guid";
import { ɵEntitySet } from "../../utilities/odata/internal/entity-set";
import { EntitySet } from "../../utilities/odata/odata.util";
import { GroupService } from "./group.service";

const cache: { [key: Guid]: Group } = {};
cache['0ea69816-9a78-4bba-a749-cacbf9a8d00d' as Guid] = {
  id: '0ea69816-9a78-4bba-a749-cacbf9a8d00d' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: undefined,
  modifiedByUserId: undefined,
  name: 'Example Group',
  description: 'I am an example group.',
};

cache['2e84fae6-d6d0-4b12-988c-87f9d4a3de7a' as Guid] = {
  id: '2e84fae6-d6d0-4b12-988c-87f9d4a3de7a' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: DateTime.now(),
  modifiedByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  name: 'Monitoring Team',
  description: 'Group responsible for monitoring system performance.',
};

cache['5cb6e49c-4f2d-4e77-bf9e-7bb4c6c1914f' as Guid] = {
  id: '5cb6e49c-4f2d-4e77-bf9e-7bb4c6c1914f' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: DateTime.now(),
  modifiedByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  name: 'Development Group',
  description: 'Developers and engineers working on system performance enhancements.',
};

cache['7af48e57-bb9f-4b5e-841c-6b9af5c5f9e2' as Guid] = {
  id: '7af48e57-bb9f-4b5e-841c-6b9af5c5f9e2' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: undefined,
  modifiedByUserId: undefined,
  name: 'Operations Group',
  description: 'Group that oversees operational performance monitoring.',
};

cache['bcf1d70a-1ab4-49e8-b9b0-5cbec9e43f90' as Guid] = {
  id: 'bcf1d70a-1ab4-49e8-b9b0-5cbec9e43f90' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: DateTime.now(),
  modifiedByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  name: 'Analytics Group',
  description: 'Group focused on analyzing performance metrics and trends.',
};

@Injectable({
  providedIn: 'root'
})
class MockGroupService extends GroupService {

  override get(id: Guid): Observable<Group> {
    const maybeRecord = cache[id];

    if (!maybeRecord) {
      return throwError(() => new Error('Record does not exist'));
    }

    return of({ ...maybeRecord }).pipe(
      delay(1000)
    );
  }

  override get set(): EntitySet<Group> {
    return new ɵEntitySet.MockImplementation<Group>(() => {
      return Object.keys(cache).map(key => cache[key as Guid]);
    });
  }

  override search(): Observable<Group[]> {
    throw new Error("Method not implemented.");
  }

  override create(record: Partial<Group>): Observable<Group> {
    record = { ...record };
    record.id = generateGuid();
    record.createdAt = DateTime.now();
    record.createdByUserId = generateGuid();

    cache[record.id] = record as Group;

    return of({ ...record } as Group).pipe(
      delay(1000)
    );
  }

  override update(id: Guid, record: Partial<Group>): Observable<Group> {
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

export function provideGroupServiceMock(): Provider {
  return {
    provide: GroupService,
    useClass: MockGroupService,
  };
}
