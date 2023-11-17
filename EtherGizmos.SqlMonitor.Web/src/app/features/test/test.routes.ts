import { Routes } from "@angular/router";

export const TEST_ROUTES: Routes = [
  { path: '', pathMatch: 'full', loadComponent: () => import('./test.component').then(m => m.TestComponent) }
];
