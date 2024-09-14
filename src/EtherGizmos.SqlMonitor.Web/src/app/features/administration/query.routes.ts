import { Routes } from "@angular/router";

export const QUERY_ROUTES: Routes = [
  { path: '', pathMatch: 'full', loadComponent: () => import('./pages/query-list/query-list.component').then(m => m.QueryListComponent) },
  { path: 'new', loadComponent: () => import('./pages/query-detail/query-detail.component').then(m => m.QueryDetailComponent) },
  { path: ':id', loadComponent: () => import('./pages/query-detail/query-detail.component').then(m => m.QueryDetailComponent) },
];
