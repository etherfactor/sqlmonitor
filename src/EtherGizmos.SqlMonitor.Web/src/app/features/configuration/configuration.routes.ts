import { Routes } from "@angular/router";

export const CONFIGURATION_ROUTES: Routes = [
  { path: '', pathMatch: 'full', loadComponent: () => import('./pages/home/home.component').then(m => m.HomeComponent) },
];
