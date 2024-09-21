import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { RouterTestingHarness } from '@angular/router/testing';
import { APP_ROUTES } from '../../../../app.routes';
import { DashboardListComponent } from './dashboard-list.component';

describe('DashboardListComponent', () => {
  let component: DashboardListComponent;
  let fixture: RouterTestingHarness;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        DashboardListComponent,
      ],
      providers: [
        provideRouter(APP_ROUTES),
      ]
    })
    .compileComponents();

    fixture = await RouterTestingHarness.create();
    component = await fixture.navigateByUrl('/dashboards', DashboardListComponent);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
