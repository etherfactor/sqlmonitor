import { Routes } from "@angular/router";

export const APP_ROUTES: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'old-dashboards' },
  { path: 'admin', loadChildren: () => import('./features/administration/administration.routes').then(m => m.ADMINISTRATION_ROUTES) },
  { path: 'dashboard', loadChildren: () => import('./features/dashboard/dashboard.routes').then(m => m.DASHBOARD_ROUTES) },
  { path: 'dashboards', loadChildren: () => import('./features/dashboard/dashboard.routes').then(m => m.DASHBOARD_ROUTES) },
  { path: 'old-dashboard', loadChildren: () => import('./features/dashboard-old/dashboard.routes').then(m => m.DASHBOARD_ROUTES) },
  { path: 'old-dashboards', loadChildren: () => import('./features/dashboard-old/dashboard.routes').then(m => m.DASHBOARD_ROUTES) },
];
