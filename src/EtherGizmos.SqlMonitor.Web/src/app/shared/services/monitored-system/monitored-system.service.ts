import { Injectable, inject } from '@angular/core';
import { signalStore } from '@ngrx/signals';
import { Observable } from 'rxjs';
import { MonitoredSystem } from '../../models/monitored-system';
import { Guid } from '../../types/guid/guid';
import { EntitySet, o } from '../../utilities/odata/odata.util';
import { withStateLoading } from '../../utilities/service-store/service-store.util';

@Injectable({
  providedIn: 'root',
  useFactory: () => { throw new Error('Abstract service, do not instantiate directly'); }
})
export abstract class MonitoredSystemService {

  constructor() { }

  abstract get(id: Guid): Observable<MonitoredSystem>;

  abstract get set(): EntitySet<MonitoredSystem>;

  abstract search(): Observable<MonitoredSystem[]>;

  abstract create(record: Partial<MonitoredSystem>): Observable<MonitoredSystem>;

  abstract update(id: Guid, record: Partial<MonitoredSystem>): Observable<MonitoredSystem>;

  abstract delete(id: Guid): Observable<void>;
}

export const MonitoredSystemStore = signalStore(
  { providedIn: 'root' },
  withStateLoading(() => {
    const $monitoredSystem = inject(MonitoredSystemService);

    return {
      states: {
        Active: {
          meta: {
            color: 'success',
            tooltip: 'Actively monitored systems',
          },
          load: $monitoredSystem.set
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
            tooltip: 'Not monitored systems',
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
      },
    };
  }),
);
