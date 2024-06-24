import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Metric } from '../../models/metric';
import { Guid } from '../../types/guid/guid';

@Injectable({
  providedIn: 'root',
  useFactory: () => { throw new Error('Abstract service, do not instantiate directly'); }
})
export abstract class MetricService {

  constructor() { }

  abstract get(id: Guid): Observable<Metric>;

  abstract search(): Observable<Metric[]>;
}
