import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { MetricData } from '../../models/metric-data';
import { Guid } from '../../types/guid/guid';

@Injectable({
  providedIn: 'root',
  useFactory: () => { throw new Error('Abstract service, do not instantiate directly'); }
})
export abstract class MetricDataService {

  constructor() { }

  abstract watchMetricData(id: Guid): Observable<MetricData>;
}
