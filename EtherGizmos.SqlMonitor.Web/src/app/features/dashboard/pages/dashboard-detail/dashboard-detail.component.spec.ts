import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { RouterTestingHarness } from '@angular/router/testing';
import { APP_ROUTES } from '../../../../app.routes';
import { provideDashboardServiceMock } from '../../../../shared/services/dashboard/dashboard.service.mock';
import { provideInstanceServiceMock } from '../../../../shared/services/instance/instance.service.mock';
import { provideMetricDataServiceMock } from '../../../../shared/services/metric-data/metric-data.service.mock';
import { provideMetricServiceMock } from '../../../../shared/services/metric/metric.service.mock';
import { DashboardDetailComponent } from './dashboard-detail.component';

describe('DashboardDetailComponent', () => {
  let component: DashboardDetailComponent;
  let fixture: RouterTestingHarness;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        DashboardDetailComponent,
      ],
      providers: [
        provideRouter(APP_ROUTES),
        provideDashboardServiceMock(),
        provideInstanceServiceMock(),
        provideMetricServiceMock(),
        provideMetricDataServiceMock(),
      ]
    })
    .compileComponents();

    fixture = await RouterTestingHarness.create();
    component = await fixture.navigateByUrl('/dashboard/05e38a95-0d4c-4527-a6a6-6b9f2c0ea0ff', DashboardDetailComponent);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
