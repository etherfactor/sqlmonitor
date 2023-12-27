import { Injectable } from '@angular/core';
import { DateTime } from 'luxon';
import { Observable, filter, map, share } from 'rxjs';
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

  //metrics: { [metricId: Guid]: MetricWatch } = {};

  //subscriptions: { [key: Guid]: MetricDataSubscription } = {};

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
      share()
    );

    //return random values
    //then interval
    //then share
  }

  startWatch(metricId: Guid): Guid {
    this.watches[metricId] ??= [];
    const metricWatches = this.watches[metricId];

    const watchId = generateGuid();
    metricWatches.push(watchId);

    return watchId;
  }

  endWatch(watchId: Guid): void {

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
      //map(data => {
      //  this.$metricDataCache.cache(data);
      //  const cachedData = this.$metricDataCache.get(data.metricId, buckets);

      //  return cachedData;
      //})
    );

    const subscription = new MetricSubscription(watchId, data$);

    return subscription;
  }

  endAllWatches(): void {

  }

  abstract watchMetricData(id: Guid): Observable<MetricData>;

  //subscribe(metricId: Guid, bucket: string | undefined): MetricDataSubscription {
  //  const watch = this.metrics[metricId] ?? {
  //    data$: this.watchMetricData(metricId),
  //    subscribers: []
  //  };
  //  this.metrics[metricId] = watch;

  //  const input$ = watch.data$.pipe(
  //    filter(data => data.bucket === bucket || bucket === undefined)
  //  );

  //  new Observable<string>()

  //  const subscription = new MetricDataSubscription(
  //    (self: MetricDataSubscription) => this.unsubscribe(self),
  //    input$,
  //    metricId,
  //    bucket
  //  );

  //  return subscription;
  //}

  //@Bound private unsubscribe(subscription: MetricDataSubscription) {

  //  //Remove the subscription
  //  const index = this.metrics[subscription.metricId].subscribers.indexOf(subscription.id);
  //  this.metrics[subscription.metricId].subscribers.splice(index, 1);
  //}
}

//interface MetricWatch {
//  data$: Observable<MetricData>;
//  subscribers: Guid[];
//}

//export class MetricDataSubscription {

//  id: Guid;

//  private unsubscribeInner: (self: MetricDataSubscription) => void;

//  allData: MetricData[] = [];
//  data$: Observable<MetricData[]>;

//  metricId: Guid;
//  bucket?: string;

//  constructor(unsubscribe: (self: MetricDataSubscription) => void, data$: Observable<MetricData>, metricId: Guid, bucket: string | undefined) {
//    this.id = generateGuid();

//    this.unsubscribeInner = unsubscribe;

//    this.data$ = data$.pipe(
//      switchMap(data => {
//        this.allData.push(data);
//        return of(this.allData);
//      })
//    );
//    this.metricId = metricId;
//    this.bucket = bucket;
//  }

//  unsubscribe() {
//    this.unsubscribeInner(this);
//  }
//}
