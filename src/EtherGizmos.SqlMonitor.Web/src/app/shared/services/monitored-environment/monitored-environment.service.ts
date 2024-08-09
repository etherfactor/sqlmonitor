import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { MonitoredEnvironment } from '../../models/monitored-environment';
import { Guid } from '../../types/guid/guid';
import { EntitySet } from '../../utilities/odata/odata.util';

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
