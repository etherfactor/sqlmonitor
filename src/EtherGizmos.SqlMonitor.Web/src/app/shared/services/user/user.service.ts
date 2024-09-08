import { Injectable, inject } from '@angular/core';
import { signalStore } from '@ngrx/signals';
import { DateTime } from 'luxon';
import { Observable } from 'rxjs';
import { User } from '../../models/user';
import { Guid } from '../../types/guid/guid';
import { EntitySet, o } from '../../utilities/odata/odata.util';
import { withStateLoading } from '../../utilities/service-store/service-store.util';

@Injectable({
  providedIn: 'root',
  useFactory: () => { throw new Error('Abstract service, do not instantiate directly'); }
})
export abstract class UserService {

  constructor() { }

  abstract get(id: Guid): Observable<User>;

  abstract get set(): EntitySet<User>;

  abstract search(): Observable<User[]>;

  abstract create(record: Partial<User>): Observable<User>;

  abstract update(id: Guid, record: Partial<User>): Observable<User>;

  abstract delete(id: Guid): Observable<void>;
}

export const UserStore = signalStore(
  { providedIn: 'root' },
  withStateLoading(() => {
    const $user = inject(UserService);

    return {
      states: {
        Active: {
          meta: {
            color: 'success',
            tooltip: 'Currently active users',
          },
          load: $user.set
            .filter(e =>
              o.and(
                o.eq(
                  e.prop('isActive'),
                  o.bool(true),
                ),
                o.ge(
                  e.prop('lastLoginAt'),
                  o.dateTime(DateTime.now().minus({ days: 30 })),
                )
              )
            )
            .top(0)
            .count()
            .execute(),
        },
        Dormant: {
          meta: {
            color: 'warning',
            tooltip: 'Users who have not logged in for over 30 days',
          },
          load:
            $user.set
              .filter(e =>
                o.and(
                  o.eq(
                    e.prop('isActive'),
                    o.bool(true),
                  ),
                  o.or(
                    o.lt(
                      e.prop('lastLoginAt'),
                      o.dateTime(DateTime.now().minus({ days: 30 })),
                    ),
                    o.eq(
                      e.prop('lastLoginAt'),
                      o.null(),
                    )
                  )
                )
              )
              .top(0)
              .count()
              .execute(),
        },
        Locked: {
          meta: {
            color: 'secondary',
            tooltip: 'Currently inactive users',
          },
          load:
            $user.set
              .filter(e =>
                o.ne(
                  e.prop('isActive'),
                  o.bool(true),
                )
              )
              .top(0)
              .count()
              .execute(),
        },
      },
    };
  }),
);
