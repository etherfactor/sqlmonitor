import { Injectable, Provider } from "@angular/core";
import { DateTime } from "luxon";
import { Observable, delay, of, throwError } from "rxjs";
import { User } from "../../models/user";
import { Guid, generateGuid } from "../../types/guid/guid";
import { ɵEntitySet } from "../../utilities/odata/internal/entity-set";
import { EntitySet } from "../../utilities/odata/odata.util";
import { UserService } from "./user.service";

const cache: { [key: Guid]: User } = {};
cache['31aa3050-a6ce-4a36-83f9-bed2f00abf97' as Guid] = {
  id: '31aa3050-a6ce-4a36-83f9-bed2f00abf97' as Guid,
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
