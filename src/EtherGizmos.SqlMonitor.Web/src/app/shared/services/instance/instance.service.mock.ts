import { Provider } from "@angular/core";
import { Observable, of, throwError } from "rxjs";
import { Instance } from "../../models/instance";
import { Guid, parseGuid } from "../../types/guid/guid";
import { InstanceService } from "./instance.service";

class MockInstanceService extends InstanceService {

  instances: Instance[];

  constructor() {
    super();

    this.instances = [
      {
        id: parseGuid('006db56a-d3b1-4c50-af8f-f19bb13f85ed'),
        name: 'SQL1',
        isActive: true,
        address: 'server1',
      },
      {
        id: parseGuid('5d469831-4c01-487d-a19b-78bdd6c99798'),
        name: 'SQL2',
        isActive: true,
        address: 'server2',
      },
      {
        id: parseGuid('d525a8e8-e06c-4351-9e5c-c29043521259'),
        name: 'SQL3',
        isActive: true,
        address: 'server3',
      },
    ];
  }

  override get(id: Guid): Observable<Instance> {
    const index = this.instances.findIndex(e => e.id === id);
    if (index >= 0) {
      return of(this.instances[index]);
    }

    return throwError(() => new Error('Instance not found.'));
  }

  override search(): Observable<Instance[]> {
    return of(this.instances);
  }
}

export function provideInstanceServiceMock(): Provider {
  return {
    provide: InstanceService,
    useFactory: () => new MockInstanceService(),
  };
}
