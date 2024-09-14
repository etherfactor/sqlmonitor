import { Injectable, Provider } from "@angular/core";
import { DateTime, Duration } from "luxon";
import { Observable, delay, of, throwError } from "rxjs";
import { Query } from "../../models/query";
import { SqlType } from "../../models/sql-type";
import { Guid, generateGuid } from "../../types/guid/guid";
import { ɵEntitySet } from "../../utilities/odata/internal/entity-set";
import { EntitySet } from "../../utilities/odata/odata.util";
import { QueryService } from "./query.service";

const cache: { [key: Guid]: Query } = {};
cache['6535522c-053d-44b8-922f-39c7457279a2' as Guid] = {
  id: '6535522c-053d-44b8-922f-39c7457279a2' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: undefined,
  modifiedByUserId: undefined,
  name: 'Example Query',
  description: 'I am an example query.',
  runFrequency: Duration.fromISOTime('00:05:00'),
  lastRunAt: DateTime.now(),
  isActive: true,
  bucketColumn: null,
  timestampUtcColumn: null,
  variants: [
    {
      sqlType: SqlType.SqlServer,
      queryText: 'select 1 as value;',
    },
  ],
  metrics: [
    {
      metricId: 1,
      valueColumn: 'value',
    },
  ],
};

cache['1e2c16b1-4e7f-40a5-bc73-4453e8d7a6ec' as Guid] = {
  id: '1e2c16b1-4e7f-40a5-bc73-4453e8d7a6ec' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: DateTime.now(),
  modifiedByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  name: 'CPU Usage Query',
  description: 'Query to retrieve CPU usage metrics.',
  runFrequency: Duration.fromISOTime('00:10:00'),
  lastRunAt: DateTime.now(),
  isActive: true,
  bucketColumn: 'cpu_bucket',
  timestampUtcColumn: 'timestamp',
  variants: [
    {
      sqlType: SqlType.MySql,
      queryText: 'SELECT AVG(cpu_usage) AS cpu_average FROM system_metrics GROUP BY cpu_bucket;',
    },
    {
      sqlType: SqlType.PostgreSql,
      queryText: 'SELECT AVG(cpu_usage) AS cpu_average FROM system_metrics GROUP BY cpu_bucket;',
    },
  ],
  metrics: [
    {
      metricId: 2,
      valueColumn: 'cpu_average',
    },
  ],
};

cache['d57b47e6-8b4b-4c3f-9b67-dae0f8963ae8' as Guid] = {
  id: 'd57b47e6-8b4b-4c3f-9b67-dae0f8963ae8' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: DateTime.now(),
  modifiedByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  name: 'Memory Usage Query',
  description: 'Query to retrieve memory usage metrics.',
  runFrequency: Duration.fromISOTime('00:15:00'),
  lastRunAt: DateTime.now(),
  isActive: true,
  bucketColumn: 'memory_bucket',
  timestampUtcColumn: 'timestamp',
  variants: [
    {
      sqlType: SqlType.MariaDb,
      queryText: 'SELECT SUM(memory_used) AS memory_total FROM memory_metrics GROUP BY memory_bucket;',
    },
    {
      sqlType: SqlType.PostgreSql,
      queryText: 'SELECT SUM(memory_used) AS memory_total FROM memory_metrics GROUP BY memory_bucket;',
    },
  ],
  metrics: [
    {
      metricId: 3,
      valueColumn: 'memory_total',
    },
  ],
};

cache['f245ba68-cb79-4d48-b6f1-123d12cf293b' as Guid] = {
  id: 'f245ba68-cb79-4d48-b6f1-123d12cf293b' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: DateTime.now(),
  modifiedByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  name: 'Disk I/O Query',
  description: 'Query to retrieve disk I/O metrics.',
  runFrequency: Duration.fromISOTime('00:30:00'),
  lastRunAt: DateTime.now(),
  isActive: true,
  bucketColumn: 'disk_bucket',
  timestampUtcColumn: 'timestamp',
  variants: [
    {
      sqlType: SqlType.SqlServer,
      queryText: 'SELECT SUM(io_read) AS total_reads, SUM(io_write) AS total_writes FROM disk_io_metrics GROUP BY disk_bucket;',
    },
    {
      sqlType: SqlType.MySql,
      queryText: 'SELECT SUM(io_read) AS total_reads, SUM(io_write) AS total_writes FROM disk_io_metrics GROUP BY disk_bucket;',
    },
  ],
  metrics: [
    {
      metricId: 4,
      valueColumn: 'total_reads',
    },
    {
      metricId: 5,
      valueColumn: 'total_writes',
    },
  ],
};

@Injectable({
  providedIn: 'root'
})
class MockQueryService extends QueryService {

  override get(id: Guid): Observable<Query> {
    const maybeRecord = cache[id];

    if (!maybeRecord) {
      return throwError(() => new Error('Record does not exist'));
    }

    return of({ ...maybeRecord }).pipe(
      delay(1000)
    );
  }

  override get set(): EntitySet<Query> {
    return new ɵEntitySet.MockImplementation<Query>(() => {
      return Object.keys(cache).map(key => cache[key as Guid]);
    });
  }

  override search(): Observable<Query[]> {
    throw new Error("Method not implemented.");
  }

  override create(record: Partial<Query>): Observable<Query> {
    record = { ...record };
    record.id = generateGuid();
    record.createdAt = DateTime.now();
    record.createdByUserId = generateGuid();

    cache[record.id] = record as Query;

    return of({ ...record } as Query).pipe(
      delay(1000)
    );
  }

  override update(id: Guid, record: Partial<Query>): Observable<Query> {
    const maybeRecord = cache[id];

    if (!maybeRecord) {
      return throwError(() => new Error('Record does not exist'));
    }

    record = { ...record };
    Object.assign(maybeRecord, record);
    cache[id] = maybeRecord;

    return of({ ...maybeRecord }).pipe(
      delay(1000)
    );
  }

  override delete(id: Guid): Observable<void> {
    const maybeRecord = cache[id];

    if (!maybeRecord) {
      return throwError(() => new Error('Record does not exist'));
    }

    delete cache[id];

    return of(void 0).pipe(
      delay(1000)
    );
  }
}

export function provideQueryServiceMock(): Provider {
  return {
    provide: QueryService,
    useClass: MockQueryService,
  };
}
