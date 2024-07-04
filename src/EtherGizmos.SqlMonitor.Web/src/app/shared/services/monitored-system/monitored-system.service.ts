import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { MonitoredSystem } from '../../models/monitored-system';
import { Guid } from '../../types/guid/guid';

@Injectable({
  providedIn: 'root',
  useFactory: () => { throw new Error('Abstract service, do not instantiate directly'); }
})
export abstract class MonitoredSystemService {

  constructor() { }

  abstract get(id: Guid): Observable<MonitoredSystem>;

  abstract search(): Observable<MonitoredSystem[]>;

  abstract create(record: MonitoredSystem): Observable<MonitoredSystem>;

  abstract update(id: Guid, record: MonitoredSystem): Observable<MonitoredSystem>;

  abstract delete(id: Guid): Observable<void>;
}
