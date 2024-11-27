import { Injectable, inject } from '@angular/core';
import { signalStore } from '@ngrx/signals';
import { Observable } from 'rxjs';
import { ScriptInterpreter } from '../../models/script-interpreter';
import { EntitySet } from '../../utilities/odata/odata.util';
import { withStateLoading } from '../../utilities/service-store/service-store.util';

@Injectable({
  providedIn: 'root',
  useFactory: () => { throw new Error('Abstract service, do not instantiate directly'); }
})
export abstract class ScriptInterpreterService {

  constructor() { }

  abstract get(id: number): Observable<ScriptInterpreter>;

  abstract get set(): EntitySet<ScriptInterpreter>;

  abstract search(): Observable<ScriptInterpreter[]>;

  abstract create(record: Partial<ScriptInterpreter>): Observable<ScriptInterpreter>;

  abstract update(id: number, record: Partial<ScriptInterpreter>): Observable<ScriptInterpreter>;

  abstract delete(id: number): Observable<void>;
}

export const ScriptStore = signalStore(
  { providedIn: 'root' },
  withStateLoading(() => {
    const $query = inject(ScriptInterpreterService);

    return {
      states: {
        Active: {
          meta: {
            color: 'success',
            tooltip: 'Currently configured script interpreters',
          },
          load: $query.set
            .top(0)
            .count()
            .execute(),
        },
      },
    };
  }),
);
