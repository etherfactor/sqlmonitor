import { Injectable, inject } from '@angular/core';
import { signalStore } from '@ngrx/signals';
import { DateTime } from 'luxon';
import { Observable } from 'rxjs';
import { Script } from '../../models/script';
import { Guid } from '../../types/guid/guid';
import { EntitySet, o } from '../../utilities/odata/odata.util';
import { withStateLoading } from '../../utilities/service-store/service-store.util';

@Injectable({
  providedIn: 'root',
  useFactory: () => { throw new Error('Abstract service, do not instantiate directly'); }
})
export abstract class ScriptService {

  constructor() { }

  abstract get(id: Guid): Observable<Script>;

  abstract get set(): EntitySet<Script>;

  abstract search(): Observable<Script[]>;

  abstract create(record: Partial<Script>): Observable<Script>;

  abstract update(id: Guid, record: Partial<Script>): Observable<Script>;

  abstract delete(id: Guid): Observable<void>;
}

export const ScriptStore = signalStore(
  { providedIn: 'root' },
  withStateLoading(() => {
    const $query = inject(ScriptService);

    return {
      states: {
        Failed: {
          meta: {
            color: 'danger',
            tooltip: 'Queries that failed to execute in the past 24 hours',
          },
          load: $query.set
            .filter(e =>
              o.and(
                o.eq(
                  e.prop('isActive'),
                  o.bool(true),
                ),
                o.ge(
                  e.prop('lastFailureAt'),
                  o.dateTime(DateTime.now().minus({ days: 1 })),
                )
              )
            )
            .top(0)
            .count()
            .execute(),
        },
        Healthy: {
          meta: {
            color: 'success',
            tooltip: 'Queries with no recent failures',
          },
          load:
            $query.set
              .filter(e =>
                o.and(
                  o.eq(
                    e.prop('isActive'),
                    o.bool(true),
                  ),
                  o.or(
                    o.lt(
                      e.prop('lastFailureAt'),
                      o.dateTime(DateTime.now().minus({ days: 1 })),
                    ),
                    o.eq(
                      e.prop('lastFailureAt'),
                      o.null()
                    )
                  )
                )
              )
              .top(0)
              .count()
              .execute(),
        },
        Inactive: {
          meta: {
            color: 'secondary',
            tooltip: 'Disabled queries',
          },
          load:
            $query.set
              .filter(e =>
                o.eq(
                  e.prop('isActive'),
                  o.bool(false),
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
