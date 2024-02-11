import { Component, OnInit } from '@angular/core';
import { NavbarMenuService } from '../../../../shared/services/navbar-menu/navbar-menu.service';
import { BodyContainerType, BodyService } from '../../../../shared/services/body/body.service';

@Component({
  selector: 'app-dashboard-list',
  standalone: true,
  imports: [],
  templateUrl: './dashboard-list.component.html',
  styleUrl: './dashboard-list.component.scss'
})
export class DashboardListComponent implements OnInit {

  private readonly $body: BodyService;
  private readonly $navbarMenu: NavbarMenuService;

  constructor(
    $body: BodyService,
    $navbarMenu: NavbarMenuService,
  ) {
    this.$body = $body;
    this.$navbarMenu = $navbarMenu;
  }

  ngOnInit(): void {
    this.$body.setContainer(BodyContainerType.Fluid);
    this.updateBreadcrumbs();
    this.updateActions();
  }

  private updateBreadcrumbs() {
    this.$navbarMenu.setBreadcrumbs([
      {
        label: 'Home',
        link: '/',
      },
      {
        label: 'Dashboards',
        link: '/dashboards',
      },
    ]);
  }

  private updateActions() {
    this.$navbarMenu.setActions([]);
  }
}
