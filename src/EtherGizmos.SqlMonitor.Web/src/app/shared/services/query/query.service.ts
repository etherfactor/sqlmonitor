import { Injectable, inject } from '@angular/core';
import { signalStore } from '@ngrx/signals';
import { Observable } from 'rxjs';
import { Query } from '../../models/query';
import { Guid } from '../../types/guid/guid';
import { EntitySet, o } from '../../utilities/odata/odata.util';
import { withStateLoading } from '../../utilities/service-store/service-store.util';

@Injectable({
  providedIn: 'root',
  useFactory: () => { throw new Error('Abstract service, do not instantiate directly'); }
})
export abstract class QueryService {
  
  constructor() { }

  abstract get(id: Guid): Observable<Query>;

  abstract get set(): EntitySet<Query>;

  abstract search(): Observable<Query[]>;

  abstract create(record: Partial<Query>): Observable<Query>;

  abstract update(id: Guid, record: Partial<Query>): Observable<Query>;

  abstract delete(id: Guid): Observable<void>;
}

export const QueryStore = signalStore(
  { providedIn: 'root' },
  withStateLoading(() => {
    const $monitoredSystem = inject(QueryService);

    return {
      states: {
        Active: {
          meta: {
            color: 'danger',
            tooltip: 'Queries that failed to execute',
          },
          load: $monitoredSystem.set
            .filter(e =>
              o.and(
                o.eq(
                  e.prop('isActive'),
                  o.bool(true),
                ),
                o.eq(
                  o.int(1),
                  o.int(0),
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
            tooltip: 'Queries that executed sucessfully',
          },
          load:
            $monitoredSystem.set
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
        Inactive: {
          meta: {
            color: 'secondary',
            tooltip: 'Disabled queries',
          },
          load:
            $monitoredSystem.set
              .filter(e =>
                o.ne(
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
