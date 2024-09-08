import { Routes } from "@angular/router";

export const USER_ROUTES: Routes = [
  { path: '', pathMatch: 'full', loadComponent: () => import('./pages/user-list/user-list.component').then(m => m.UserListComponent) },
  { path: 'new', loadComponent: () => import('./pages/user-detail/user-detail.component').then(m => m.UserDetailComponent) },
  { path: ':id', loadComponent: () => import('./pages/user-detail/user-detail.component').then(m => m.UserDetailComponent) },
];
