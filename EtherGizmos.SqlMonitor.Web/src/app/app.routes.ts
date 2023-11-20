import { Routes } from "@angular/router";

export const APP_ROUTES: Routes = [
  { path: '', pathMatch: 'full', loadChildren: () => import('./features/dashboard/dashboard.routes').then(m => m.DASHBOARD_ROUTES) }
];
