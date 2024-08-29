import { computed } from "@angular/core";
import { patchState, signalStoreFeature, withComputed, withMethods, withState } from "@ngrx/signals";
import { DateTime } from "luxon";
import { Observable } from "rxjs";
import { ODataResultSet } from "../odata/odata.util";

export interface RecordStatus extends RecordStatusMeta {
  label: string;
  count: number;
}

export interface RecordStatusMeta {
  color: string;
  tooltip?: string;
}

export const maxSummaryAge = 15 * 60 * 1000;

export function createState(states: { [label: string]: RecordStatusMeta }) {
  return {
    _loadingDepth: 0,
    _meta: {
      totalLastUpdated: undefined as DateTime | undefined,
    },
    _stateMetaMap: states,
    _stateCountMap: {} as { [label: string]: number },
  };
}

interface StateLoadingOptions<TEntity> {
  states: {
    [label: string]: {
      meta: RecordStatusMeta,
      load: Observable<ODataResultSet<TEntity>>,
    }
  },
}

export function withStateLoading<TEntity>(optionsBuilder: () => StateLoadingOptions<TEntity>) {
  return signalStoreFeature(
    withState(() => {
      const options = optionsBuilder();

      const states: { [label: string]: RecordStatusMeta } = {};
      const keys = Object.keys(options.states);
      for (const key of keys) {
        states[key] = options.states[key].meta;
      }

      return createState(states);
    }),
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
      const options = optionsBuilder();
      const keys = Object.keys(options.states);

      return {
        loadTotals(force?: 'force'): void {
          const lastUpdated = store._meta.totalLastUpdated();
          if (force !== 'force' && lastUpdated && lastUpdated.toMillis() + maxSummaryAge >= DateTime.now().toMillis())
            return;

          for (const key of keys) {
            const load = options.states[key].load;
            store._startLoading();
            load.subscribe(result => {
              store._setStateCount(key, result["@odata.count"] ?? 0);
              store._finishLoading();
            });
          }
        },
      };
    }),
  );
}
