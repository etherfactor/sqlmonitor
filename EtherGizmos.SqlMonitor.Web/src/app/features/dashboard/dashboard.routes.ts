import { Routes } from "@angular/router";

export const DASHBOARD_ROUTES: Routes = [
  { path: '', pathMatch: 'full', loadComponent: () => import('./components/index/dashboard.component').then(m => m.DashboardComponent) }
];
