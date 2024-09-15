/// <reference types="@angular/localize" />

import { CDK_DRAG_CONFIG } from '@angular/cdk/drag-drop';
import { InjectionToken, Provider, enableProdMode, importProvidersFrom } from '@angular/core';
import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { provideCharts, withDefaultRegisterables } from 'ng2-charts';
import { provideEnvironmentNgxMask } from 'ngx-mask';
import { MonacoEditorModule } from 'ngx-monaco-editor-v2';
import { provideQuillConfig } from 'ngx-quill';
import { AppComponent } from './app/app.component';
import { APP_ROUTES } from './app/app.routes';
import { provideDashboardServiceMock } from './app/shared/services/dashboard/dashboard.service.mock';
import { provideGroupServiceMock } from './app/shared/services/group/group.service.mock';
import { provideInstanceServiceMock } from './app/shared/services/instance/instance.service.mock';
import { provideMetricDataServiceMock } from './app/shared/services/metric-data/metric-data.service.mock';
import { provideMetricServiceMock } from './app/shared/services/metric/metric.service.mock';
import { provideMonitoredEnvironmentServiceMock } from './app/shared/services/monitored-environment/monitored-environment.mock';
import { provideMonitoredResourceServiceMock } from './app/shared/services/monitored-resource/monitored-resource.service.mock';
import { provideMonitoredSystemServiceMock } from './app/shared/services/monitored-system/monitored-system.service.mock';
import { provideQueryServiceMock } from './app/shared/services/query/query.service.mock';
import { provideUserServiceMock } from './app/shared/services/user/user.service.mock';
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
      provideCharts(withDefaultRegisterables()),
      provideSimpleConfig(CDK_DRAG_CONFIG, { zIndex: 1100 }),
      provideEnvironmentNgxMask(),
      importProvidersFrom(MonacoEditorModule.forRoot()),
      provideDashboardServiceMock(),
      provideGroupServiceMock(),
      provideInstanceServiceMock(),
      provideMetricServiceMock(),
      provideMetricDataServiceMock(),
      provideMonitoredEnvironmentServiceMock(),
      provideMonitoredResourceServiceMock(),
      provideMonitoredSystemServiceMock(),
      provideQueryServiceMock(),
      provideUserServiceMock(),
    ]
  })
  .catch(err => console.error(err));

function provideSimpleConfig<TConfig>(token: InjectionToken<TConfig>, value: TConfig): Provider {
  return {
    provide: token,
    useValue: value,
  };
}
