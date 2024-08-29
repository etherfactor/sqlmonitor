import { Injectable, inject } from '@angular/core';
import { signalStore } from '@ngrx/signals';
import { Observable } from 'rxjs';
import { MonitoredResource } from '../../models/monitored-resource';
import { Guid } from '../../types/guid/guid';
import { EntitySet, o } from '../../utilities/odata/odata.util';
import { withStateLoading } from '../../utilities/service-store/service-store.util';

@Injectable({
  providedIn: 'root',
  useFactory: () => { throw new Error('Abstract service, do not instantiate directly'); }
})
export abstract class MonitoredResourceService {

  constructor() { }

  abstract get(id: Guid): Observable<MonitoredResource>;

  abstract get set(): EntitySet<MonitoredResource>;

  abstract search(): Observable<MonitoredResource[]>;

  abstract create(record: Partial<MonitoredResource>): Observable<MonitoredResource>;

  abstract update(id: Guid, record: Partial<MonitoredResource>): Observable<MonitoredResource>;

  abstract delete(id: Guid): Observable<void>;
}

export const MonitoredResourceStore = signalStore(
  { providedIn: 'root' },
  withStateLoading(() => {
    const $monitoredResource = inject(MonitoredResourceService);

    return {
      states: {
        Active: {
          meta: {
            color: 'success',
            tooltip: 'Actively monitored resources',
          },
          load: $monitoredResource.set
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
            tooltip: 'Not monitored resources',
          },
          load:
            $monitoredResource.set
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
