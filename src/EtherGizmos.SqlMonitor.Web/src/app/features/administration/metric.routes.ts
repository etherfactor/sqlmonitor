import { Routes } from '@angular/router';

export const METRIC_ROUTES: Routes = [
  { path: '', pathMatch: 'full', loadComponent: () => import('./pages/metric-list/metric-list.component').then(m => m.MetricListComponent) },
  { path: 'new', loadComponent: () => import('./pages/metric-detail/metric-detail.component').then(m => m.MetricDetailComponent) },
  { path: ':id', loadComponent: () => import('./pages/metric-detail/metric-detail.component').then(m => m.MetricDetailComponent) },
];
