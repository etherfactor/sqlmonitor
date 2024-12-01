import { Routes } from "@angular/router";

export const SCRIPT_INTERPRETER_ROUTES: Routes = [
  { path: '', pathMatch: 'full', loadComponent: () => import('./pages/script-interpreter-list/script-interpreter-list.component').then(m => m.ScriptInterpreterListComponent) },
  { path: 'new', loadComponent: () => import('./pages/script-interpreter-detail/script-interpreter-detail.component').then(m => m.ScriptInterpreterDetailComponent) },
  { path: ':id', loadComponent: () => import('./pages/script-interpreter-detail/script-interpreter-detail.component').then(m => m.ScriptInterpreterDetailComponent) },
];
