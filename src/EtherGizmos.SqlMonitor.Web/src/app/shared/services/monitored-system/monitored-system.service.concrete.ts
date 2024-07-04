import { HttpClient } from "@angular/common/http";
import { Injectable, Provider } from "@angular/core";
import { Observable } from "rxjs";
import { MonitoredSystem } from "../../models/monitored-system";
import { Guid } from "../../types/guid/guid";
import { MonitoredSystemService } from "./monitored-system.service";

@Injectable({
  providedIn: 'root'
})
class ConcreteMonitoredSystemService extends MonitoredSystemService {
  private readonly $http: HttpClient;

  constructor(
    $http: HttpClient,
  ) {
    super();
    this.$http = $http;
  }

  override get(id: Guid): Observable<MonitoredSystem> {
    throw new Error("Method not implemented.");
  }

  override search(): Observable<MonitoredSystem[]> {
    throw new Error("Method not implemented.");
  }
}

export function provideMonitoredSystemService(): Provider {
  return {
    provide: MonitoredSystemService,
    useClass: ConcreteMonitoredSystemService,
  };
}
