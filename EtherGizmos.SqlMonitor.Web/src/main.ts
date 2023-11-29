import { enableProdMode } from '@angular/core';
import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { provideQuillConfig } from 'ngx-quill';
import { AppComponent } from './app/app.component';
import { APP_ROUTES } from './app/app.routes';
import { environment } from './environments/environment';

if (environment.production) {
  enableProdMode();
}

bootstrapApplication(
  AppComponent,
  {
    providers: [
      provideRouter(APP_ROUTES),
      provideQuillConfig({}),
    ]
  })
  .catch(err => console.error(err));
