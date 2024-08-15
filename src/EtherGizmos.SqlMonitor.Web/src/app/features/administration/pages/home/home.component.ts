import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, signal } from '@angular/core';
import { RouterModule } from '@angular/router';
import { BodyContainerType, BodyService } from '../../../../shared/services/body/body.service';
import { MonitoredEnvironmentService } from '../../../../shared/services/monitored-environment/monitored-environment.service';
import { MonitoredResourceService } from '../../../../shared/services/monitored-resource/monitored-resource.service';
import { MonitoredSystemService } from '../../../../shared/services/monitored-system/monitored-system.service';
import { NavbarMenuAction, NavbarMenuBreadcrumb, NavbarMenuService } from '../../../../shared/services/navbar-menu/navbar-menu.service';
import { o } from '../../../../shared/utilities/odata/odata.util';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {

  private readonly $body: BodyService;
  private readonly $monitoredEnvironment: MonitoredEnvironmentService;
  private readonly $monitoredResource: MonitoredResourceService;
  private readonly $monitoredSystem: MonitoredSystemService;
  private readonly $navbarMenu: NavbarMenuService;

  loadingEnvironments$$ = computed(() => this.activeEnvironments$$() === undefined || this.inactiveEnvironments$$() === undefined);
  activeEnvironments$$ = signal<number | undefined>(undefined);
  inactiveEnvironments$$ = signal<number | undefined>(undefined);
  totalEnvironments$$ = computed(() => (this.activeEnvironments$$() ?? 0) + (this.inactiveEnvironments$$() ?? 0));

  constructor(
    $body: BodyService,
    $monitoredEnvironment: MonitoredEnvironmentService,
    $monitoredResource: MonitoredResourceService,
    $monitoredSystem: MonitoredSystemService,
    $navbarMenu: NavbarMenuService,
  ) {
    this.$body = $body;
    this.$monitoredEnvironment = $monitoredEnvironment;
    this.$monitoredResource = $monitoredResource;
    this.$monitoredSystem = $monitoredSystem;
    this.$navbarMenu = $navbarMenu;
  }

  ngOnInit(): void {
    this.$body.setContainer(BodyContainerType.Normal);
    this.$navbarMenu.setActions(this.actions);
    this.$navbarMenu.setBreadcrumbs(this.breadcrumbs);

    this.$monitoredEnvironment.set
      .filter(e =>
        o.eq(
          e.prop('isActive'),
          o.bool(true),
        ),
      )
      .top(0)
      .count()
      .execute()
      .subscribe(aEnv => this.activeEnvironments$$.set(aEnv['@odata.count']));

    this.$monitoredEnvironment.set
      .filter(e =>
        o.ne(
          e.prop('isActive'),
          o.bool(true),
        )
      )
      .top(0)
      .count()
      .execute()
      .subscribe(iEnv => this.inactiveEnvironments$$.set(iEnv['@odata.count']));
  }

  protected get actions(): NavbarMenuAction[] {
    const actions: NavbarMenuAction[] = [];

    return actions;
  }

  protected get breadcrumbs(): NavbarMenuBreadcrumb[] {
    const breadcrumbs: NavbarMenuBreadcrumb[] = [
      {
        label: 'Home',
        link: '/',
      },
      {
        label: 'Administration',
        link: '/admin',
      },
    ];

    return breadcrumbs;
  }
}
