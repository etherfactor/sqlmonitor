import { Routes } from "@angular/router";

export const ADMINISTRATION_ROUTES: Routes = [
  { path: '', pathMatch: 'full', loadComponent: () => import('./pages/home/home.component').then(m => m.HomeComponent) },
  { path: 'user', loadChildren: () => import('./user.routes').then(m => m.USER_ROUTES) },
  { path: 'users', loadChildren: () => import('./user.routes').then(m => m.USER_ROUTES) },
  { path: 'group', loadChildren: () => import('./group.routes').then(m => m.GROUP_ROUTES) },
  { path: 'groups', loadChildren: () => import('./group.routes').then(m => m.GROUP_ROUTES) },
  { path: 'query', loadChildren: () => import('./query.routes').then(m => m.QUERY_ROUTES) },
  { path: 'queries', loadChildren: () => import('./query.routes').then(m => m.QUERY_ROUTES) },
  { path: 'system', loadChildren: () => import('./monitored-system.routes').then(m => m.MONITORED_SYSTEM_ROUTES) },
  { path: 'systems', loadChildren: () => import('./monitored-system.routes').then(m => m.MONITORED_SYSTEM_ROUTES) },
  { path: 'resource', loadChildren: () => import('./monitored-resource.routes').then(m => m.MONITORED_RESOURCE_ROUTES) },
  { path: 'resources', loadChildren: () => import('./monitored-resource.routes').then(m => m.MONITORED_RESOURCE_ROUTES) },
  { path: 'environment', loadChildren: () => import('./monitored-environment.routes').then(m => m.MONITORED_ENVIRONMENT_ROUTES) },
  { path: 'environments', loadChildren: () => import('./monitored-environment.routes').then(m => m.MONITORED_ENVIRONMENT_ROUTES) },
];
