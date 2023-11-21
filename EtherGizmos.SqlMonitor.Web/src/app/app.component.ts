import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NavbarMenuAction, NavbarMenuCallback, NavbarMenuService } from './shared/services/navbar-menu/navbar-menu.service';

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

  constructor($navbarMenu: NavbarMenuService) {
    this.$navbarMenu = $navbarMenu;
  }

  ngOnInit(): void {
    this.$navbarMenu.actions$.subscribe(actions => {
      console.log('from', this.actions);
      this.actions = actions;
      console.log('to', this.actions);
    });
  }
  
  performCallback(action: NavbarMenuCallback) {
    action.callback?.();
  }
}
