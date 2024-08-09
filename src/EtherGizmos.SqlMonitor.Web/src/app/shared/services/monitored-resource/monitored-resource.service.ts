import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { MonitoredResource } from '../../models/monitored-resource';
import { Guid } from '../../types/guid/guid';
import { EntitySet } from '../../utilities/odata/odata.util';

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
