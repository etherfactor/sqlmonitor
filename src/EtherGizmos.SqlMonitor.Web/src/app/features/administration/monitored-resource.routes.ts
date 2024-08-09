import { Routes } from "@angular/router";

export const MONITORED_RESOURCE_ROUTES: Routes = [
  { path: '', pathMatch: 'full', loadComponent: () => import('./pages/monitored-resource-list/monitored-resource-list.component').then(m => m.MonitoredResourceListComponent) },
  { path: 'new', loadComponent: () => import('./pages/monitored-resource-detail/monitored-resource-detail.component').then(m => m.MonitoredResourceDetailComponent) },
  { path: ':id', loadComponent: () => import('./pages/monitored-resource-detail/monitored-resource-detail.component').then(m => m.MonitoredResourceDetailComponent) },
];
