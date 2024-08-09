import { Routes } from "@angular/router";

export const MONITORED_ENVIRONMENT_ROUTES: Routes = [
  { path: '', pathMatch: 'full', loadComponent: () => import('./pages/monitored-environment-list/monitored-environment-list.component').then(m => m.MonitoredEnvironmentListComponent) },
  { path: 'new', loadComponent: () => import('./pages/monitored-environment-detail/monitored-environment-detail.component').then(m => m.MonitoredEnvironmentDetailComponent) },
  { path: ':id', loadComponent: () => import('./pages/monitored-environment-detail/monitored-environment-detail.component').then(m => m.MonitoredEnvironmentDetailComponent) },
];
