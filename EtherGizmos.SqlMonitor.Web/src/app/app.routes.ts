import { Routes } from "@angular/router";

export const APP_ROUTES: Routes = [
  { path: '', pathMatch: 'full', loadComponent: () => import('./app.component').then(m => m.AppComponent) },
  { path: 'test', loadChildren: () => import('./features/test/test.routes').then(m => m.TEST_ROUTES) }
];
