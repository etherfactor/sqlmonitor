import { Injectable, Provider } from "@angular/core";
import { DateTime } from "luxon";
import { Observable, delay, of, throwError } from "rxjs";
import { ScriptInterpreter } from "../../models/script-interpreter";
import { Guid, generateGuid } from "../../types/guid/guid";
import { ɵEntitySet } from "../../utilities/odata/internal/entity-set";
import { EntitySet } from "../../utilities/odata/odata.util";
import { ScriptInterpreterService } from "./script-interpreter.service";

let increment = 0;

const cache: { [key: number]: ScriptInterpreter } = {};
cache[++increment] = {
  id: increment,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: undefined,
  modifiedByUserId: undefined,
  name: 'PowerShell 7',
  description: 'PowerShell 7 is a cross-platform scripting language compatible with Windows, macOS, and Linux. It runs scripts using the "pwsh" command, offering enhanced performance, modern language features, and improved compatibility with modules and scripts across different platforms.',
  command: 'pwsh',
  arguments: '-NoProfile -ExecutionPolicy Unrestricted -Command ./$Script',
  extension: 'ps1',
  monacoLanguage: 'powershell',
};

cache[++increment] = {
  id: increment,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: undefined,
  modifiedByUserId: undefined,
  name: 'PowerShell 5',
  description: 'PowerShell 5 is a Windows-native scripting language bundled with Windows operating systems. It executes scripts using the "powershell" command, providing comprehensive Windows systems management capabilities through its extensive commandlets library.',
  command: 'powershell',
  arguments: '-NoProfile -ExecutionPolicy Unrestricted -Command $Script',
  extension: 'ps1',
  monacoLanguage: 'powershell',
};

cache[++increment] = {
  id: increment,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: undefined,
  modifiedByUserId: undefined,
  name: 'Bash',
  description: 'Bash is a widely-used Unix shell and command language available on most Unix-based systems, including macOS and Linux. It executes scripts using the "bash" command, providing powerful command-line capabilities and scripting functionalities for automating tasks, file management, and system administration tasks.',
  command: 'bash',
  arguments: '-c $Script',
  extension: 'sh',
  monacoLanguage: 'shell',
};

cache[++increment] = {
  id: increment,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: undefined,
  modifiedByUserId: undefined,
  name: 'Python',
  description: 'Python is a versatile, high-level programming language known for its readability and broad range of applications. It executes scripts using the "python" or "python3" command, making it ideal for automation, data analysis, web development, and more.',
  command: 'python3',
  arguments: '$Script',
  extension: 'py',
  monacoLanguage: 'python',
};

cache[++increment] = {
  id: increment,
  createdAt: DateTime.now(),
  createdByUserId: 'df2aa7a9-16bb-4403-bb60-6bc809d6894a' as Guid,
  modifiedAt: undefined,
  modifiedByUserId: undefined,
  name: 'Node.js',
  description: 'Node.js is a JavaScript runtime built on Chrome\'s V8 engine. It executes scripts using the "node" command, enabling server-side scripting and the development of scalable network applications.',
  command: 'node',
  arguments: '$Script',
  extension: 'js',
  monacoLanguage: 'javascript',
};

@Injectable({
  providedIn: 'root'
})
class MockScriptInterpreterService extends ScriptInterpreterService {

  override get(id: number): Observable<ScriptInterpreter> {
    const maybeRecord = cache[id];

    if (!maybeRecord) {
      return throwError(() => new Error('Record does not exist'));
    }

    return of({ ...maybeRecord }).pipe(
      delay(1000)
    );
  }

  override get set(): EntitySet<ScriptInterpreter> {
    return new ɵEntitySet.MockImplementation<ScriptInterpreter>(() => {
      return Object.keys(cache).map(key => cache[key as unknown as number]);
    });
  }

  override search(): Observable<ScriptInterpreter[]> {
    throw new Error("Method not implemented.");
  }

  override create(record: Partial<ScriptInterpreter>): Observable<ScriptInterpreter> {
    record = { ...record };
    record.id = ++increment;
    record.createdAt = DateTime.now();
    record.createdByUserId = generateGuid();

    cache[record.id] = record as ScriptInterpreter;

    return of({ ...record } as ScriptInterpreter).pipe(
      delay(1000)
    );
  }

  override update(id: number, record: Partial<ScriptInterpreter>): Observable<ScriptInterpreter> {
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

  override delete(id: number): Observable<void> {
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

export function provideScriptInterpreterServiceMock(): Provider {
  return {
    provide: ScriptInterpreterService,
    useClass: MockScriptInterpreterService,
  };
}
