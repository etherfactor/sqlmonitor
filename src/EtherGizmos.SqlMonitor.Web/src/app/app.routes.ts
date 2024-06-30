import { Routes } from "@angular/router";

export const APP_ROUTES: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'dashboards' },
  { path: 'monitored-system', loadChildren: () => import('./features/configuration/monitored-system.routes').then(m => m.MONITORED_SYSTEM_ROUTES) },
  { path: 'monitored-systems', loadChildren: () => import('./features/configuration/monitored-system.routes').then(m => m.MONITORED_SYSTEM_ROUTES) },
  { path: 'dashboard', loadChildren: () => import('./features/dashboard/dashboard.routes').then(m => m.DASHBOARD_ROUTES) },
  { path: 'dashboards', loadChildren: () => import('./features/dashboard/dashboard.routes').then(m => m.DASHBOARD_ROUTES) },
];
