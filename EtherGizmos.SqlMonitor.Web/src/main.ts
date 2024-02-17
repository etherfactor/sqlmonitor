/// <reference types="@angular/localize" />

import { CDK_DRAG_CONFIG } from '@angular/cdk/drag-drop';
import { InjectionToken, Provider, enableProdMode } from '@angular/core';
import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { provideQuillConfig } from 'ngx-quill';
import { AppComponent } from './app/app.component';
import { APP_ROUTES } from './app/app.routes';
import { provideDashboardServiceMock } from './app/shared/services/dashboard/dashboard.service.mock';
import { provideInstanceServiceMock } from './app/shared/services/instance/instance.service.mock';
import { provideMetricDataServiceMock } from './app/shared/services/metric-data/metric-data.service.mock';
import { provideMetricServiceMock } from './app/shared/services/metric/metric.service.mock';
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
      provideSimpleConfig(CDK_DRAG_CONFIG, { zIndex: 1100 }),
      provideDashboardServiceMock(),
      provideInstanceServiceMock(),
      provideMetricServiceMock(),
      provideMetricDataServiceMock(),
    ]
  })
  .catch(err => console.error(err));

function provideSimpleConfig<TConfig>(token: InjectionToken<TConfig>, value: TConfig): Provider {
  return {
    provide: token,
    useValue: value,
  };
}
