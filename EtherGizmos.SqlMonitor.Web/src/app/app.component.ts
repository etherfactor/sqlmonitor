import { CommonModule } from '@angular/common';
import { Component, OnInit, QueryList, ViewChildren } from '@angular/core';
import { RouterModule } from '@angular/router';
import { Observable, of } from 'rxjs';
import { v4 as uuidv4 } from 'uuid';
import { NavbarActionSearchDirective } from './shared/directives/navbar-action-search/navbar-action-search.directive';
import { BodyContainerType, BodyService } from './shared/services/body/body.service';
import { NavbarMenuAction, NavbarMenuBreadcrumb, NavbarMenuCallback, NavbarMenuService, NavbarMenuSubAction } from './shared/services/navbar-menu/navbar-menu.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    NavbarActionSearchDirective,
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  private readonly $body: BodyService;
  private readonly $navbarMenu: NavbarMenuService;

  @ViewChildren(NavbarActionSearchDirective) searchActions: QueryList<NavbarActionSearchDirective> = undefined!;

  title = 'EtherGizmos.SqlMonitor.Web';

  containerClass: string = BodyContainerType.Normal;

  actions: NavbarMenuAction[] = [];
  breadcrumbs: NavbarMenuBreadcrumb[] = [];

  constructor(
    $body: BodyService,
    $navbarMenu: NavbarMenuService,
  ) {
    this.$body = $body;
    this.$navbarMenu = $navbarMenu;
  }

  ngOnInit(): void {
    this.$body.containerClass$.subscribe(containerClass => {
      this.containerClass = containerClass;
    });

    this.$navbarMenu.actions$.subscribe(actions => {
      this.actions = actions;
    });

    this.$navbarMenu.breadcrumbs$.subscribe(breadcrumbs => {
      this.breadcrumbs = breadcrumbs;
    });
  }
  
  performCallback(action: NavbarMenuCallback) {

    const searchAction = this.searchActions.find(e => e.searchAction === action);
    if (searchAction) {
      searchAction.focus();
    }

    action.callback?.();
  }

  identifyNavbarAction(_: number, action: NavbarMenuAction) {
    return action.label;
  }

  identifyNavbarSubAction(_: number, action: NavbarMenuSubAction) {
    return action.label ?? uuidv4();
  }

  safe(label: string | Observable<string> | undefined): Observable<string> | undefined {
    if (label === undefined || label === null) {
      return undefined;
    } else if (label instanceof Observable) {
      return label;
    } else {
      return of(label);
    }
  }
}
