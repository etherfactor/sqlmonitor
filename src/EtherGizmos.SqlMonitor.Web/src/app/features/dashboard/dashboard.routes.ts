import { Routes } from "@angular/router";

export const DASHBOARD_ROUTES: Routes = [
  { path: ':id', pathMatch: 'full', loadComponent: () => import('./pages/dashboard-detail/dashboard-detail.component').then(m => m.DashboardDetailComponent) },
];
