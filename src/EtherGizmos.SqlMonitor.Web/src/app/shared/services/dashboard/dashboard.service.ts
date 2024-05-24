import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Dashboard } from '../../../features/dashboard/models/dashboard';
import { Guid } from '../../types/guid/guid';

@Injectable({
  providedIn: 'root',
  useFactory: () => { throw new Error('Abstract service, do not instantiate directly'); }
})
export abstract class DashboardService {

  constructor() { }

  abstract get(id: Guid): Observable<Dashboard>;

  abstract search(): Observable<Dashboard[]>;
}
