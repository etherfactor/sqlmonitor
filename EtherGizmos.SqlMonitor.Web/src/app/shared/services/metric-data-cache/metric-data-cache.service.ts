import { Injectable } from '@angular/core';
import { DateTime } from 'luxon';
import { interval } from 'rxjs';
import { MetricData } from '../../models/metric-data';
import { Guid } from '../../types/guid/guid';

@Injectable({
  providedIn: 'root'
})
export class MetricDataCacheService {

  private caches: {
    [metricId: Guid]: {
      [bucket: string]: MetricData[]
    }
  } = {};

  constructor() {
    interval(15000).subscribe(() => this.purgeOldData());
  }

  cache(data: MetricData): void {
    const metricCache = this.caches[data.metricId] ??= {};
    const bucketCache = metricCache[data.bucket ?? ''] ??= [];

    bucketCache.push(data);
  }

  get(metricId: Guid, buckets: (string | undefined)[] | undefined): MetricData[] {
    const metricCache = this.caches[metricId];
    if (!metricCache)
      return [];

    const result: MetricData[] = [];
    for (const bucket of buckets ?? Object.keys(metricCache)) {
      const bucketCache = metricCache[bucket ?? ''];
      if (bucketCache !== undefined && bucketCache !== null) {
        result.push(...bucketCache);
      }
    }

    return result;
  }

  private purgeOldData(): void {
    const now = DateTime.utc();

    for (const metricId of Object.keys(this.caches) as Guid[]) {
      const metricCache = this.caches[metricId];

      for (const bucket of Object.keys(metricCache)) {
        const bucketCache = metricCache[bucket];

        if (bucketCache) {
          // Filter out data older than 1 hour
          metricCache[bucket] = bucketCache.filter(data =>
            now.diff(data.eventTimeUtc).as('minutes') <= 1
          );
        }
      }
    }
  }
}
