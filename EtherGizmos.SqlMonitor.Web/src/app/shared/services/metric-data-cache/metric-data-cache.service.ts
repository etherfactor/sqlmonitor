import { Injectable } from '@angular/core';
import { MetricData } from '../../models/metric-data';
import { Guid } from '../../types/guid/guid';

@Injectable({
  providedIn: 'root'
})
export class MetricDataCacheService {

  private caches: {
    [metricId: Guid]: {
      [bucketId: string]: MetricData[]
    }
  } = {};

  constructor() { }

  cache(data: MetricData): void {
    const metricCache = this.caches[data.metricId] ??= {};
    const bucketCache = metricCache[data.bucket ?? ''] ??= [];

    bucketCache.push(data);
    //bucketCache.sort((a, b) => {
    //  if (a.eventTimeUtc < b.eventTimeUtc)
    //    return -1;

    //  if (a.eventTimeUtc > b.eventTimeUtc)
    //    return 1;

    //  return 0;
    //});
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
}
