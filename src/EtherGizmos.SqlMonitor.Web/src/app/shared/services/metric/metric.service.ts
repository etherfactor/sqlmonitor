import { Injectable, inject } from '@angular/core';
import { signalStore } from '@ngrx/signals';
import { Observable } from 'rxjs';
import { Metric } from '../../models/metric';
import { EntitySet, o } from '../../utilities/odata/odata.util';
import { withStateLoading } from '../../utilities/service-store/service-store.util';

@Injectable({
  providedIn: 'root',
  useFactory: () => { throw new Error('Abstract service, do not instantiate directly'); }
})
export abstract class MetricService {

  constructor() { }

  abstract get(id: number): Observable<Metric>;

  abstract get set(): EntitySet<Metric>;

  abstract search(): Observable<Metric[]>;

  abstract create(record: Partial<Metric>): Observable<Metric>;

  abstract update(id: number, record: Partial<Metric>): Observable<Metric>;

  abstract delete(id: number): Observable<void>;
}

export const MonitoredEnvironmentStore = signalStore(
  { providedIn: 'root' },
  withStateLoading(() => {
    const $metric = inject(MetricService);

    return {
      states: {
        Active: {
          meta: {
            color: 'success',
            tooltip: 'Actively monitored metrics',
          },
          load: $metric.set
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
            tooltip: 'Not monitored metrics',
          },
          load:
            $metric.set
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
