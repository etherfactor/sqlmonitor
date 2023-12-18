import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Metric } from '../../models/metric';

@Injectable({
  providedIn: 'root',
  useFactory: () => { throw new Error('Abstract service, do not instantiate directly'); }
})
export abstract class MetricService {

  constructor() { }

  abstract search(): Observable<Metric[]>;
}
