import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { AppComponent } from './app.component';
import { APP_ROUTES } from './app.routes';
import { BodyContainerType, BodyService } from './shared/services/body/body.service';
import { NavbarMenuService } from './shared/services/navbar-menu/navbar-menu.service';

describe('AppComponent', () => {
  let fixture: ComponentFixture<AppComponent>;
  let app: AppComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        AppComponent,
        RouterTestingModule.withRoutes(APP_ROUTES),
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AppComponent);
    app = fixture.componentInstance;

    const router = TestBed.inject(Router);
    await router.navigate(['/']);
    fixture.detectChanges();
  });

  it('should create the app', () => {
    expect(app).toBeTruthy();
  });

  it(`should have as title 'EtherGizmos.SqlMonitor.Web'`, () => {
    expect(app.title).toEqual('EtherGizmos.SqlMonitor.Web');
  });

  it('should update body when BodyService is called', () => {
    const $body = TestBed.inject(BodyService);
    const bodyContainer = fixture.debugElement.query(By.css('[data-testid="body-container"]'));

    expect(bodyContainer.classes['container']).toBeTruthy();

    $body.setContainer(BodyContainerType.Fluid);
    fixture.detectChanges();

    expect(bodyContainer.classes['container-fluid']).toBeFalsy();

    $body.setContainer(BodyContainerType.Normal);
    fixture.detectChanges();

    expect(bodyContainer.classes['container']).toBeTruthy();
  });

  it('should update navbar actions when NavbarMenuService is called', fakeAsync(() => {
    const $navbarMenu = TestBed.inject(NavbarMenuService);
    const navbarSub = fixture.debugElement.query(By.css('[data-testid="navbar-sub"]'));

    let callbackFired = false;
    $navbarMenu.setActions([
      {
        label: 'Action 1',
        callback: () => callbackFired = true,
      },
      {
        label: 'Action 2',
      },
    ]);
    tick();
    fixture.detectChanges();

    const actionButtons = navbarSub.queryAll(By.css('[data-testid="action-button"]'));
    expect(actionButtons.length).toBe(2 * 2);

    actionButtons[0].nativeElement.click();
    fixture.detectChanges();

    expect(callbackFired).toBe(true);
  }));

  it('should update navbar breadcrumbs when NavbarMenuService is called', fakeAsync(() => {
    const $navbarMenu = TestBed.inject(NavbarMenuService);
    const navbarSub = fixture.debugElement.query(By.css('[data-testid="navbar-sub"]'));

    $navbarMenu.setBreadcrumbs([
      {
        label: 'Breadcrumb 1',
        link: '/link'
      },
      {
        label: 'Breadcrumb 2',
        link: '/link/2'
      },
    ]);
    tick();
    fixture.detectChanges();

    const breadcrumbItems = navbarSub.queryAll(By.css('[data-testid="breadcrumb-item"]'));
    expect(breadcrumbItems.length).toBe(2);
  }));
});
