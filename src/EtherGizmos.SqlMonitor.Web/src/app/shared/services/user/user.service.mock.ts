import { Injectable, Provider } from "@angular/core";
import { DateTime } from "luxon";
import { Observable, delay, of, throwError } from "rxjs";
import { User } from "../../models/user";
import { Guid, generateGuid } from "../../types/guid/guid";
import { ɵEntitySet } from "../../utilities/odata/internal/entity-set";
import { EntitySet } from "../../utilities/odata/odata.util";
import { UserService } from "./user.service";

const cache: { [key: Guid]: User } = {};
cache['df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid] = {
  id: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: undefined,
  modifiedByUserId: undefined,
  name: 'Example User',
  username: 'user123',
  password: '***',
  emailAddress: 'user@domain.com',
  isEmailValidated: true,
  isActive: true,
  lastLoginAt: DateTime.now().minus({ days: 1 }),
  lastPasswordChangeAt: DateTime.now().minus({ days: 7 }),
};

cache['2b09c2c1-490c-45c1-b75d-86cce5d182e3' as Guid] = {
  id: '2b09c2c1-490c-45c1-b75d-86cce5d182e3' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: DateTime.now(),
  modifiedByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  name: 'Alice Smith',
  username: 'alice.smith',
  password: '***',
  emailAddress: 'alice.smith@domain.com',
  isEmailValidated: true,
  isActive: true,
  lastLoginAt: DateTime.now().minus({ days: 5 }),
  lastPasswordChangeAt: DateTime.now().minus({ days: 10 }),
};

cache['3c2d47e6-1a5b-4f56-8e4f-e2b2fc7ef69f' as Guid] = {
  id: '3c2d47e6-1a5b-4f56-8e4f-e2b2fc7ef69f' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: DateTime.now(),
  modifiedByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  name: 'Bob Johnson',
  username: 'bob.j',
  password: '***',
  emailAddress: 'bob.j@domain.com',
  isEmailValidated: false,
  isActive: true,
  lastLoginAt: DateTime.now().minus({ days: 25 }),
  lastPasswordChangeAt: DateTime.now().minus({ days: 30 }),
};

cache['4d5e70b7-201e-4e5f-bb0c-82a32521a254' as Guid] = {
  id: '4d5e70b7-201e-4e5f-bb0c-82a32521a254' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: undefined,
  modifiedByUserId: undefined,
  name: 'Charlie Brown',
  username: 'charlie.b',
  password: '***',
  emailAddress: 'charlie.b@domain.com',
  isEmailValidated: true,
  isActive: false,
  lastLoginAt: DateTime.now().minus({ days: 15 }),
  lastPasswordChangeAt: DateTime.now().minus({ days: 60 }),
};

cache['5e6b0b3d-4d49-442b-a4bb-bb61d9cfa61e' as Guid] = {
  id: '5e6b0b3d-4d49-442b-a4bb-bb61d9cfa61e' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: DateTime.now(),
  modifiedByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  name: 'Diana Prince',
  username: 'diana.p',
  password: '***',
  emailAddress: 'diana.p@domain.com',
  isEmailValidated: true,
  isActive: true,
  lastLoginAt: DateTime.now().minus({ days: 35 }),
  lastPasswordChangeAt: DateTime.now().minus({ days: 75 }),
};

@Injectable({
  providedIn: 'root'
})
class MockUserService extends UserService {

  override get(id: Guid): Observable<User> {
    const maybeRecord = cache[id];

    if (!maybeRecord) {
      return throwError(() => new Error('Record does not exist'));
    }

    return of({ ...maybeRecord }).pipe(
      delay(1000)
    );
  }

  override get set(): EntitySet<User> {
    return new ɵEntitySet.MockImplementation<User>(() => {
      return Object.keys(cache).map(key => cache[key as Guid]);
    });
  }

  override search(): Observable<User[]> {
    throw new Error("Method not implemented.");
  }

  override create(record: Partial<User>): Observable<User> {
    record = { ...record };
    record.id = generateGuid();
    record.createdAt = DateTime.now();
    record.createdByUserId = generateGuid();

    cache[record.id] = record as User;

    return of({ ...record } as User).pipe(
      delay(1000)
    );
  }

  override update(id: Guid, record: Partial<User>): Observable<User> {
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

export function provideUserServiceMock(): Provider {
  return {
    provide: UserService,
    useClass: MockUserService,
  };
}
