import { Injectable } from '@angular/core';
import { DateTime } from 'luxon';
import { Observable, filter, share, tap } from 'rxjs';
import { MetricData } from '../../models/metric-data';
import { MetricSubscription } from '../../models/metric-subscription';
import { SeverityType } from '../../models/severity-type';
import { Guid, generateGuid, parseGuid } from '../../types/guid/guid';
import { MetricDataCacheService } from '../metric-data-cache/metric-data-cache.service';

@Injectable({
  providedIn: 'root',
  useFactory: () => { throw new Error('Abstract service, do not instantiate directly'); }
})
export abstract class MetricDataService {

  private $metricDataCache: MetricDataCacheService;

  watches: { [metricId: Guid]: Guid[] } = {};
  allMetrics$: Observable<MetricData>;

  constructor(
    $metricDataCache: MetricDataCacheService
  ) {
    this.$metricDataCache = $metricDataCache;

    this.allMetrics$ = new Observable((observer) => {
      const intervalMs = 1000;

      const intervalId = setInterval(() => {
        const allMetricIds = Object.keys(this.watches).map(item => parseGuid(item));
        const distinctMetricIds = allMetricIds.filter((item, index) => allMetricIds.indexOf(item) === index);

        for (const metricId of distinctMetricIds) {
          const randomMultiplier = 1;

          observer.next({
            instanceId: parseGuid('00000000-0000-0000-0000-000000000000'),
            metricId: metricId,
            eventTimeUtc: DateTime.now(),
            bucket: 'A',
            severityType: SeverityType.Nominal,
            value: Math.random() * randomMultiplier,
          });

          observer.next({
            instanceId: parseGuid('00000000-0000-0000-0000-000000000000'),
            metricId: metricId,
            eventTimeUtc: DateTime.now(),
            bucket: 'B',
            severityType: SeverityType.Nominal,
            value: Math.random() * randomMultiplier,
          });

          observer.next({
            instanceId: parseGuid('00000000-0000-0000-0000-000000000000'),
            metricId: metricId,
            eventTimeUtc: DateTime.now(),
            bucket: 'C',
            severityType: SeverityType.Nominal,
            value: Math.random() * randomMultiplier,
          });

          observer.next({
            instanceId: parseGuid('00000000-0000-0000-0000-000000000000'),
            metricId: metricId,
            eventTimeUtc: DateTime.now(),
            bucket: 'D',
            severityType: SeverityType.Nominal,
            value: Math.random() * randomMultiplier,
          });

          observer.next({
            instanceId: parseGuid('00000000-0000-0000-0000-000000000000'),
            metricId: metricId,
            eventTimeUtc: DateTime.now(),
            bucket: 'E',
            severityType: SeverityType.Nominal,
            value: Math.random() * randomMultiplier,
          });
        }
      }, intervalMs);

      return () => {
        clearInterval(intervalId);
      };
    });

    this.allMetrics$ = this.allMetrics$.pipe(
      tap(data => this.$metricDataCache.cache(data)),
      share()
    );
  }

  startWatch(metricId: Guid): Guid {
    this.watches[metricId] ??= [];
    const metricWatches = this.watches[metricId];

    const watchId = generateGuid();
    metricWatches.push(watchId);

    return watchId;
  }

  endWatch(metricId: Guid, watchId: Guid): void {
    this.watches[metricId] ??= [];
    const metricWatches = this.watches[metricId];

    const index = metricWatches.indexOf(watchId);
    if (index >= 0) {
      metricWatches.splice(index, 1);
    }

    if (metricWatches.length === 0) {
      delete this.watches[metricId];
    }
  }

  subscribe(metricId: Guid, buckets: (string | undefined)[] | undefined): MetricSubscription {
    const watchId = this.startWatch(metricId);
    const data$ = this.allMetrics$.pipe(
      filter(data => data.metricId === metricId),
      filter(data => {
        if (!buckets)
          return true;

        for (const bucket of buckets) {
          if (data.bucket?.trim()?.toLowerCase() === bucket?.trim()?.toLowerCase()) {
            return true;
          }
        }

        return false;
      }),
    );

    const subscription = new MetricSubscription(this, watchId, metricId, data$);

    return subscription;
  }

  endAllWatches(): void {
    this.watches = {};
  }
}
