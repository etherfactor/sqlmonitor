import { Injectable, inject } from '@angular/core';
import { signalStore } from '@ngrx/signals';
import { Observable } from 'rxjs';
import { Group } from '../../models/group';
import { Guid } from '../../types/guid/guid';
import { EntitySet, o } from '../../utilities/odata/odata.util';
import { withStateLoading } from '../../utilities/service-store/service-store.util';

@Injectable({
  providedIn: 'root',
  useFactory: () => { throw new Error('Abstract service, do not instantiate directly'); }
})
export abstract class GroupService {

  constructor() { }

  abstract get(id: Guid): Observable<Group>;

  abstract get set(): EntitySet<Group>;

  abstract search(): Observable<Group[]>;

  abstract create(record: Partial<Group>): Observable<Group>;

  abstract update(id: Guid, record: Partial<Group>): Observable<Group>;

  abstract delete(id: Guid): Observable<void>;
}

export const GroupStore = signalStore(
  { providedIn: 'root' },
  withStateLoading(() => {
    const $Group = inject(GroupService);

    return {
      states: {
        Populated: {
          meta: {
            color: 'success',
            tooltip: 'Groups with at least one user',
          },
          load: $Group.set
            .filter(e =>
              o.bool(true)
            )
            .top(0)
            .count()
            .execute(),
        },
        Empty: {
          meta: {
            color: 'secondary',
            tooltip: 'Groups with no users',
          },
          load:
            $Group.set
              .filter(e =>
                o.bool(false)
              )
              .top(0)
              .count()
              .execute(),
        }
      },
    };
  }),
);
