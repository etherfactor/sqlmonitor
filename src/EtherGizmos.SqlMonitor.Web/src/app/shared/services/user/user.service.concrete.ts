import { HttpClient } from "@angular/common/http";
import { Injectable, Provider } from "@angular/core";
import { Observable } from "rxjs";
import { User } from "../../models/user";
import { Guid } from "../../types/guid/guid";
import { UserService } from "./user.service";

@Injectable({
  providedIn: 'root'
})
class ConcreteUserService extends UserService {
  private readonly $http: HttpClient;

  constructor(
    $http: HttpClient,
  ) {
    super();
    this.$http = $http;
  }

  override get(id: Guid): Observable<User> {
    throw new Error("Method not implemented.");
  }

  override search(): Observable<User[]> {
    throw new Error("Method not implemented.");
  }
}

export function provideUserService(): Provider {
  return {
    provide: UserService,
    useClass: ConcreteUserService,
  };
}
