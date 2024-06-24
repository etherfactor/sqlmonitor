import { Injectable } from '@angular/core';
import { DateTime } from 'luxon';
import { asyncScheduler, interval, observeOn } from 'rxjs';
import { v4 as uuidv4 } from 'uuid';
import { MetricData } from '../../models/metric-data';
import { Guid } from '../../types/guid/guid';

@Injectable({
  providedIn: 'root'
})
export class MetricDataCacheService {

  retentions: { [key: Guid]: DateTime } = {};

  private caches: {
    [metricId: Guid]: {
      [bucket: string]: MetricData[]
    }
  } = {};

  constructor() {
    interval(15000).pipe(
      observeOn(asyncScheduler),
    ).subscribe(() => this.purgeOldData());
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

  requestRetention(earliestTime: DateTime): () => void {
    const key = uuidv4() as Guid;
    this.retentions[key] = earliestTime;

    return () => {
      delete this.retentions[key];
    };
  }

  private purgeOldData(): void {
    //console.log('retentions', this.retentions);

    const earliestMilliseconds = Object.values(this.retentions)
      .reduce((current, next) =>
        Math.min(current, next.toMillis()),
        Number.MAX_VALUE);

    //console.log('purge earlier than', earliestMilliseconds);

    for (const metricId of Object.keys(this.caches) as Guid[]) {
      const metricCache = this.caches[metricId];

      for (const bucket of Object.keys(metricCache)) {
        const bucketCache = metricCache[bucket];

        if (bucketCache) {
          // Filter out data older than 1 hour
          metricCache[bucket] = bucketCache.filter(data =>
            data.eventTimeUtc.toMillis() < earliestMilliseconds
          );
        }
      }
    }
  }
}
