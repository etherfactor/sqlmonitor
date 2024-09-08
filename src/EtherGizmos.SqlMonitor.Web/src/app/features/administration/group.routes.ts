import { Routes } from '@angular/router';

export const GROUP_ROUTES: Routes = [
  { path: '', pathMatch: 'full', loadComponent: () => import('./pages/group-list/group-list.component').then(m => m.GroupListComponent) },
  { path: 'new', loadComponent: () => import('./pages/group-detail/group-detail.component').then(m => m.GroupDetailComponent) },
  { path: ':id', loadComponent: () => import('./pages/group-detail/group-detail.component').then(m => m.GroupDetailComponent) },
];
