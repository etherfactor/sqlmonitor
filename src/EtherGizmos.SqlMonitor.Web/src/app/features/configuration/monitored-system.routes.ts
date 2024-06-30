import { Routes } from "@angular/router";

export const MONITORED_SYSTEM_ROUTES: Routes = [
  { path: '', pathMatch: 'full', loadComponent: () => import('./pages/monitored-system-list/monitored-system-list.component').then(m => m.MonitoredSystemListComponent) },
  { path: ':id', pathMatch: 'full', loadComponent: () => import('./pages/monitored-system-detail/monitored-system-detail.component').then(m => m.MonitoredSystemDetailComponent) },
];
