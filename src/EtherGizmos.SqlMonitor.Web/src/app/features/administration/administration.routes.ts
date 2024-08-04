import { Routes } from "@angular/router";

export const ADMINISTRATION_ROUTES: Routes = [
  { path: '', pathMatch: 'full', loadComponent: () => import('./pages/home/home.component').then(m => m.HomeComponent) },
  { path: 'system', loadChildren: () => import('./monitored-system.routes').then(m => m.MONITORED_SYSTEM_ROUTES) },
  { path: 'systems', loadChildren: () => import('./monitored-system.routes').then(m => m.MONITORED_SYSTEM_ROUTES) },
];
