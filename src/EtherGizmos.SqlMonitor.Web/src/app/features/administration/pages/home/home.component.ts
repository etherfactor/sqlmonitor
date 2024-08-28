import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ActivityCardComponent } from '../../../../shared/components/activity-card/activity-card.component';
import { BodyContainerType, BodyService } from '../../../../shared/services/body/body.service';
import { MonitoredEnvironmentService, MonitoredEnvironmentStore } from '../../../../shared/services/monitored-environment/monitored-environment.service';
import { MonitoredResourceService } from '../../../../shared/services/monitored-resource/monitored-resource.service';
import { MonitoredSystemService } from '../../../../shared/services/monitored-system/monitored-system.service';
import { NavbarMenuAction, NavbarMenuBreadcrumb, NavbarMenuService } from '../../../../shared/services/navbar-menu/navbar-menu.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    ActivityCardComponent,
    CommonModule,
    RouterModule,
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {

  private readonly $body: BodyService;
  private readonly $monitoredEnvironment: MonitoredEnvironmentService;
  private readonly $monitoredEnvironmentStore = inject(MonitoredEnvironmentStore);
  private readonly $monitoredResource: MonitoredResourceService;
  private readonly $monitoredSystem: MonitoredSystemService;
  private readonly $navbarMenu: NavbarMenuService;

  loadingEnvironments$$ = this.$monitoredEnvironmentStore.isLoading;
  environmentStatuses$$ = this.$monitoredEnvironmentStore.states;

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

    this.$monitoredEnvironmentStore.loadTotals();
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
