import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ActivityCardComponent } from '../../../../shared/components/activity-card/activity-card.component';
import { BodyContainerType, BodyService } from '../../../../shared/services/body/body.service';
import { GroupStore } from '../../../../shared/services/group/group.service';
import { MonitoredEnvironmentService, MonitoredEnvironmentStore } from '../../../../shared/services/monitored-environment/monitored-environment.service';
import { MonitoredResourceService, MonitoredResourceStore } from '../../../../shared/services/monitored-resource/monitored-resource.service';
import { MonitoredSystemService, MonitoredSystemStore } from '../../../../shared/services/monitored-system/monitored-system.service';
import { NavbarMenuAction, NavbarMenuBreadcrumb, NavbarMenuService } from '../../../../shared/services/navbar-menu/navbar-menu.service';
import { UserStore } from '../../../../shared/services/user/user.service';
import { QueryStore } from '../../../../shared/services/query/query.service';

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
  private readonly $groupStore = inject(GroupStore);
  private readonly $monitoredEnvironment: MonitoredEnvironmentService;
  private readonly $monitoredEnvironmentStore = inject(MonitoredEnvironmentStore);
  private readonly $monitoredResource: MonitoredResourceService;
  private readonly $monitoredResourceStore = inject(MonitoredResourceStore);
  private readonly $monitoredSystem: MonitoredSystemService;
  private readonly $monitoredSystemStore = inject(MonitoredSystemStore);
  private readonly $queryStore = inject(QueryStore);
  private readonly $userStore = inject(UserStore);
  private readonly $navbarMenu: NavbarMenuService;

  loadingGroups$$ = this.$groupStore.isLoading;
  groupStatuses$$ = this.$groupStore.states;

  loadingEnvironments$$ = this.$monitoredEnvironmentStore.isLoading;
  environmentStatuses$$ = this.$monitoredEnvironmentStore.states;

  loadingResources$$ = this.$monitoredResourceStore.isLoading;
  resourceStatuses$$ = this.$monitoredResourceStore.states;

  loadingSystems$$ = this.$monitoredSystemStore.isLoading;
  systemStatuses$$ = this.$monitoredSystemStore.states;

  loadingQueries$$ = this.$queryStore.isLoading;
  queryStatuses$$ = this.$queryStore.states;

  loadingUsers$$ = this.$userStore.isLoading;
  userStatuses$$ = this.$userStore.states;

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

    this.$groupStore.loadTotals();
    this.$monitoredEnvironmentStore.loadTotals();
    this.$monitoredResourceStore.loadTotals();
    this.$monitoredSystemStore.loadTotals();
    this.$queryStore.loadTotals();
    this.$userStore.loadTotals();
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
