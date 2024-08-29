import { Injectable, inject } from '@angular/core';
import { signalStore } from '@ngrx/signals';
import { Observable } from 'rxjs';
import { MonitoredEnvironment } from '../../models/monitored-environment';
import { Guid } from '../../types/guid/guid';
import { EntitySet, o } from '../../utilities/odata/odata.util';
import { withStateLoading } from '../../utilities/service-store/service-store.util';

@Injectable({
  providedIn: 'root',
  useFactory: () => { throw new Error('Abstract service, do not instantiate directly'); }
})
export abstract class MonitoredEnvironmentService {

  constructor() { }

  abstract get(id: Guid): Observable<MonitoredEnvironment>;

  abstract get set(): EntitySet<MonitoredEnvironment>;

  abstract search(): Observable<MonitoredEnvironment[]>;

  abstract create(record: Partial<MonitoredEnvironment>): Observable<MonitoredEnvironment>;

  abstract update(id: Guid, record: Partial<MonitoredEnvironment>): Observable<MonitoredEnvironment>;

  abstract delete(id: Guid): Observable<void>;
}

export const MonitoredEnvironmentStore = signalStore(
  { providedIn: 'root' },
  withStateLoading(() => {
    const $monitoredEnvironment = inject(MonitoredEnvironmentService);

    return {
      states: {
        Active: {
          meta: {
            color: 'success',
            tooltip: 'Actively monitored environments',
          },
          load: $monitoredEnvironment.set
            .filter(e =>
              o.eq(
                e.prop('isActive'),
                o.bool(true),
              ),
            )
            .top(0)
            .count()
            .execute(),
        },
        Inactive: {
          meta: {
            color: 'secondary',
            tooltip: 'Not monitored environments',
          },
          load:
            $monitoredEnvironment.set
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
