import { Routes } from "@angular/router";

export const APP_ROUTES: Routes = [
  { path: '', pathMatch: 'full', loadComponent: () => import('./features/test/test.component').then(m => m.TestComponent) },
  { path: 'test', loadChildren: () => import('./features/test/test.routes').then(m => m.TEST_ROUTES) }
];
