import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NavbarMenuBreadcrumb, NavbarMenuAction, NavbarMenuCallback, NavbarMenuService } from './shared/services/navbar-menu/navbar-menu.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  private $navbarMenu: NavbarMenuService;

  title = 'EtherGizmos.SqlMonitor.Web';
  actions: NavbarMenuAction[] = [];
  breadcrumbs: NavbarMenuBreadcrumb[] = [];

  constructor($navbarMenu: NavbarMenuService) {
    this.$navbarMenu = $navbarMenu;
  }

  ngOnInit(): void {
    this.$navbarMenu.actions$.subscribe(actions => {
      this.actions = actions;
    });

    this.$navbarMenu.breadcrumbs$.subscribe(breadcrumbs => {
      this.breadcrumbs = breadcrumbs;
    });
  }
  
  performCallback(action: NavbarMenuCallback) {
    action.callback?.();
  }
}
