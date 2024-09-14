import { HttpClient } from "@angular/common/http";
import { Injectable, Provider } from "@angular/core";
import { Observable } from "rxjs";
import { Query } from "../../models/query";
import { Guid } from "../../types/guid/guid";
import { QueryService } from "./query.service";

@Injectable({
  providedIn: 'root'
})
class ConcreteQueryService extends QueryService {
  private readonly $http: HttpClient;

  constructor(
    $http: HttpClient,
  ) {
    super();
    this.$http = $http;
  }

  override get(id: Guid): Observable<Query> {
    throw new Error("Method not implemented.");
  }

  override search(): Observable<Query[]> {
    throw new Error("Method not implemented.");
  }
}

export function provideQueryService(): Provider {
  return {
    provide: QueryService,
    useClass: ConcreteQueryService,
  };
}
