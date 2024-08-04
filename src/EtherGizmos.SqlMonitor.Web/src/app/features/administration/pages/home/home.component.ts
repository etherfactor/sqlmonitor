import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { BodyContainerType, BodyService } from '../../../../shared/services/body/body.service';
import { NavbarMenuAction, NavbarMenuBreadcrumb, NavbarMenuService } from '../../../../shared/services/navbar-menu/navbar-menu.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    RouterModule,
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {

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
    this.$body.setContainer(BodyContainerType.Normal);
    this.$navbarMenu.setActions(this.actions);
    this.$navbarMenu.setBreadcrumbs(this.breadcrumbs);
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
