import { Routes } from "@angular/router";

export const DASHBOARD_ROUTES: Routes = [
  { path: '', pathMatch: 'full', loadComponent: () => import('./components/dashboard-list/dashboard-list.component').then(m => m.DashboardListComponent) },
  { path: ':id', pathMatch: 'full', loadComponent: () => import('./components/dashboard-detail/dashboard-detail.component').then(m => m.DashboardDetailComponent) },
];
