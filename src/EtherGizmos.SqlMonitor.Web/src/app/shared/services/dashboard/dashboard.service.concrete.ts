import { HttpClient } from '@angular/common/http';
import { Injectable, Provider } from '@angular/core';
import { Observable } from 'rxjs';
import { Dashboard } from '../../../features/dashboard/models/dashboard';
import { Guid } from '../../types/guid/guid';
import { DashboardService } from './dashboard.service';

@Injectable({
  providedIn: 'root'
})
class ConcreteDashboardService extends DashboardService {

  private $http: HttpClient;

  constructor(
    $http: HttpClient,
  ) {
    super();
    this.$http = $http;
  }

  override get(id: Guid): Observable<Dashboard> {
    throw new Error("Method not implemented.");
  }

  override search(): Observable<Dashboard[]> {
    throw new Error("Method not implemented.");
  }
}

export function provideDashboardService(): Provider {
  return {
    provide: DashboardService,
    useFactory: (
      $http: HttpClient,
    ) => new ConcreteDashboardService(
      $http,
    )
  };
}
