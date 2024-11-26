import { Injectable, Provider } from "@angular/core";
import { DateTime, Duration } from "luxon";
import { Observable, delay, of, throwError } from "rxjs";
import { Script } from "../../models/script";
import { Guid, generateGuid } from "../../types/guid/guid";
import { ɵEntitySet } from "../../utilities/odata/internal/entity-set";
import { EntitySet } from "../../utilities/odata/odata.util";
import { ScriptService } from "./script.service";

const cache: { [key: Guid]: Script } = {};
cache['46fcabc9-4f5c-4de9-b176-5b0e25053f4e' as Guid] = {
  id: '46fcabc9-4f5c-4de9-b176-5b0e25053f4e' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: undefined,
  modifiedByUserId: undefined,
  name: 'Example Script',
  description: 'I am an example script.',
  runFrequency: Duration.fromISOTime('00:05:00'),
  lastRunAt: DateTime.now(),
  lastFailureAt: undefined,
  isActive: true,
  bucketColumn: null,
  timestampUtcColumn: null,
  variants: [
    {
      scriptInterpreterId: 1, //1: PowerShell 7; 2: PowerShell 5; 3: Bash
      scriptText: 'Write-Host "##metric value=1 bucket=`"Test`"";',
    },
  ],
  metrics: [
    {
      metricId: 1,
      valueKey: 'value',
    },
  ],
};

cache['f2524ce7-542e-4dbf-a5e5-0081103b476b' as Guid] = {
  id: 'f2524ce7-542e-4dbf-a5e5-0081103b476b' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: DateTime.now(),
  modifiedByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  name: 'CPU Usage Monitor',
  description: 'Monitors CPU usage every 10 minutes.',
  runFrequency: Duration.fromISOTime('00:10:00'),
  lastRunAt: DateTime.now(),
  lastFailureAt: DateTime.now().minus({ minutes: 15 }),
  isActive: true,
  bucketColumn: 'server',
  timestampUtcColumn: 'timestamp',
  variants: [
    {
      scriptInterpreterId: 2, //PowerShell 5
      scriptText: 'Write-Host "##metric cpuUsage=75 bucket=`"Server01`"";',
    },
  ],
  metrics: [
    {
      metricId: 2,
      valueKey: 'cpuUsage',
    },
  ],
};

cache['154dfe4d-b0f1-4920-b1f8-6a864cb265f7' as Guid] = {
  id: '154dfe4d-b0f1-4920-b1f8-6a864cb265f7' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: undefined,
  modifiedByUserId: undefined,
  name: 'Disk Space Monitor',
  description: 'Checks disk space usage every 15 minutes.',
  runFrequency: Duration.fromISOTime('00:15:00'),
  lastRunAt: DateTime.now(),
  lastFailureAt: undefined,
  isActive: false,
  bucketColumn: 'disk',
  timestampUtcColumn: 'timestamp',
  variants: [
    {
      scriptInterpreterId: 3, //Bash
      scriptText: 'echo "##metric diskFree=120 bucket=`/dev/sda1`";',
    },
  ],
  metrics: [
    {
      metricId: 3,
      valueKey: 'diskFree',
    },
  ],
};

cache['3495d382-5178-4a5e-8294-35871aecf754' as Guid] = {
  id: '3495d382-5178-4a5e-8294-35871aecf754' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: DateTime.now(),
  modifiedByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  name: 'Memory Usage Monitor',
  description: 'Tracks memory usage every 5 minutes.',
  runFrequency: Duration.fromISOTime('00:05:00'),
  lastRunAt: DateTime.now(),
  lastFailureAt: undefined,
  isActive: false,
  bucketColumn: 'server',
  timestampUtcColumn: 'timestamp',
  variants: [
    {
      scriptInterpreterId: 1, //PowerShell 7
      scriptText: 'Write-Host "##metric memoryUsage=65 bucket=`"Server02`"";',
    },
  ],
  metrics: [
    {
      metricId: 4,
      valueKey: 'memoryUsage',
    },
  ],
};

cache['73b339fd-7b6f-43d9-a060-d5fa27356740' as Guid] = {
  id: '73b339fd-7b6f-43d9-a060-d5fa27356740' as Guid,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: undefined,
  modifiedByUserId: undefined,
  name: 'Network Latency Monitor',
  description: 'Measures network latency every 20 minutes.',
  runFrequency: Duration.fromISOTime('00:20:00'),
  lastRunAt: DateTime.now(),
  lastFailureAt: DateTime.now().minus({ days: 7 }),
  isActive: true,
  bucketColumn: 'region',
  timestampUtcColumn: 'timestamp',
  variants: [
    {
      scriptInterpreterId: 3, //Bash
      scriptText: 'echo "##metric latency=30 bucket=`us-east`";',
    },
  ],
  metrics: [
    {
      metricId: 5,
      valueKey: 'latency',
    },
  ],
};

@Injectable({
  providedIn: 'root'
})
class MockScriptService extends ScriptService {

  override get(id: Guid): Observable<Script> {
    const maybeRecord = cache[id];

    if (!maybeRecord) {
      return throwError(() => new Error('Record does not exist'));
    }

    return of({ ...maybeRecord }).pipe(
      delay(1000)
    );
  }

  override get set(): EntitySet<Script> {
    return new ɵEntitySet.MockImplementation<Script>(() => {
      return Object.keys(cache).map(key => cache[key as Guid]);
    });
  }

  override search(): Observable<Script[]> {
    throw new Error("Method not implemented.");
  }

  override create(record: Partial<Script>): Observable<Script> {
    record = { ...record };
    record.id = generateGuid();
    record.createdAt = DateTime.now();
    record.createdByUserId = generateGuid();

    cache[record.id] = record as Script;

    return of({ ...record } as Script).pipe(
      delay(1000)
    );
  }

  override update(id: Guid, record: Partial<Script>): Observable<Script> {
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

export function provideScriptServiceMock(): Provider {
  return {
    provide: ScriptService,
    useClass: MockScriptService,
  };
}
