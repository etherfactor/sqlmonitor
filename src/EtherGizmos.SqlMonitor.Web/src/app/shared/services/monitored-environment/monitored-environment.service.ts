import { Injectable, computed, inject } from '@angular/core';
import { patchState, signalStore, withComputed, withMethods, withState } from '@ngrx/signals';
import { DateTime } from 'luxon';
import { Observable } from 'rxjs';
import { RecordStatus } from '../../components/activity-card/activity-card.component';
import { MonitoredEnvironment } from '../../models/monitored-environment';
import { Guid } from '../../types/guid/guid';
import { FilterColumnCondition } from '../../utilities/filter/filter.util';
import { EntitySet, o } from '../../utilities/odata/odata.util';
import { SortColumn } from '../../utilities/sort/sort.util';

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

interface RecordStatusMeta {
  color: string,
  tooltip?: string,
}

const maxAge = 15 * 60 * 1000;

const initialState = {
  _loadingDepth: 0,
  meta: {
    filter: [] as FilterColumnCondition[],
    sort: [] as SortColumn[],
    page: 1,
  },
  _meta: {
    totalLastUpdated: undefined as DateTime | undefined,
  },
  //recordMap: {} as Record<Guid, MonitoredEnvironment>,
  _stateMetaMap: {
    Active: { color: 'success', tooltip: 'Currently active environments' },
    Inactive: { color: 'secondary', tooltip: 'Not monitored environments' },
  } as { [label: string]: RecordStatusMeta },
  _stateCountMap: {} as { [label: string]: number },
};

export const MonitoredEnvironmentStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withComputed(store => {
    return {
      isLoading: computed(() => store._loadingDepth() > 0),
      states: computed(() => {
        const stateMetaMap = store._stateMetaMap();
        const stateCountMap = store._stateCountMap();
        const keys = Object.keys(stateMetaMap);
        
        const states: RecordStatus[] = [];
        for (const key of keys) {
          const state = stateMetaMap[key];
          const recordState: RecordStatus = { ...state, label: key, count: stateCountMap[key] ?? 0 };
          states.push(recordState);
        }

        return states;
      }),
    };
  }),
  withComputed(store => {
    return {
      stateTotal: computed(() => store.states().reduce((total, current) => total + current.count, 0)),
    };
  }),
  withMethods(store => {
    return {
      _startLoading(): void {
        patchState(store, { _loadingDepth: store._loadingDepth() + 1 });
      },
      _finishLoading(): void {
        patchState(store, { _loadingDepth: store._loadingDepth() - 1 });
      },
      _setStateCount(label: string, count: number): void {
        patchState(store, {
          _stateCountMap: { ...store._stateCountMap(), [label]: count },
          _meta: { ...store._meta, totalLastUpdated: DateTime.now() },
        });
      },
    };
  }),
  withMethods(store => {
    const $monitoredEnvironment = inject(MonitoredEnvironmentService);

    return {
      loadTotals(force?: 'force'): void {
        const lastUpdated = store._meta.totalLastUpdated();
        console.log(lastUpdated?.toMillis() ?? 0 + maxAge);
        console.log(DateTime.now().toMillis());
        if (force !== 'force' && lastUpdated && lastUpdated.toMillis() + maxAge >= DateTime.now().toMillis())
          return;
          
        store._startLoading();
        $monitoredEnvironment.set
          .filter(e =>
            o.eq(
              e.prop('isActive'),
              o.bool(true),
            ),
          )
          .top(0)
          .count()
          .execute()
          .subscribe(active => {
            store._setStateCount('Active', active['@odata.count'] ?? 0);
            store._finishLoading();
          });

        store._startLoading();
        $monitoredEnvironment.set
          .filter(e =>
            o.ne(
              e.prop('isActive'),
              o.bool(true),
            )
          )
          .top(0)
          .count()
          .execute()
          .subscribe(inactive => {
            store._setStateCount('Inactive', inactive['@odata.count'] ?? 0);
            store._finishLoading();
          });
      }
    };
  }),
);
