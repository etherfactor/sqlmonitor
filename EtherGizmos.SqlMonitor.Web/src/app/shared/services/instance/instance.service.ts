import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Instance } from '../../models/instance';
import { Guid } from '../../types/guid/guid';

@Injectable({
  providedIn: 'root',
  useFactory: () => { throw new Error('Abstract service, do not instantiate directly'); }
})
export abstract class InstanceService {

  constructor() { }

  abstract get(id: Guid): Observable<Instance>;

  abstract search(): Observable<Instance[]>;
}
