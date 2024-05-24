import { HttpClient } from "@angular/common/http";
import { Provider } from "@angular/core";
import { Observable } from "rxjs";
import { Instance } from "../../models/instance";
import { Guid } from "../../types/guid/guid";
import { InstanceService } from "./instance.service";

class ConcreteInstanceService extends InstanceService {

  private $http: HttpClient;

  constructor(
    $http: HttpClient,
  ) {
    super();
    this.$http = $http;
  }

  override get(id: Guid): Observable<Instance> {
    throw new Error("Method not implemented.");
  }

  override search(): Observable<Instance[]> {
    throw new Error("Method not implemented.");
  }
}

export function provideInstanceService(): Provider {
  return {
    provide: InstanceService,
    useFactory: (
      $http: HttpClient,
    ) => new ConcreteInstanceService(
      $http,
    )
  };
}
