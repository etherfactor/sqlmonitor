import { Routes } from "@angular/router";

export const SCRIPT_ROUTES: Routes = [
  { path: '', pathMatch: 'full', loadComponent: () => import('./pages/script-list/script-list.component').then(m => m.ScriptListComponent) },
  { path: 'new', loadComponent: () => import('./pages/script-detail/script-detail.component').then(m => m.ScriptDetailComponent) },
  { path: ':id', loadComponent: () => import('./pages/script-detail/script-detail.component').then(m => m.ScriptDetailComponent) },
];
