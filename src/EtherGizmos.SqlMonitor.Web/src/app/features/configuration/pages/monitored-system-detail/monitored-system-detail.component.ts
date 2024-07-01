import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { NgSelectModule } from '@ng-select/ng-select';
import { BodyContainerType, BodyService } from '../../../../shared/services/body/body.service';
import { NavbarMenuService } from '../../../../shared/services/navbar-menu/navbar-menu.service';

@Component({
  selector: 'app-monitored-system-detail',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    NgSelectModule,
  ],
  templateUrl: './monitored-system-detail.component.html',
  styleUrl: './monitored-system-detail.component.scss'
})
export class MonitoredSystemDetailComponent implements OnInit {

  private readonly $activatedRoute: ActivatedRoute;
  private readonly $body: BodyService;
  private readonly $form: FormBuilder;
  private readonly $navbarMenu: NavbarMenuService;

  constructor(
    $activatedRoute: ActivatedRoute,
    $body: BodyService,
    $form: FormBuilder,
    $navbarMenu: NavbarMenuService,
  ) {
    this.$activatedRoute = $activatedRoute;
    this.$body = $body;
    this.$form = $form;
    this.$navbarMenu = $navbarMenu;
  }

  ngOnInit(): void {
    this.$body.setContainer(BodyContainerType.Normal);
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
        label: 'Monitored Systems',
        link: '/monitored-systems',
      },
      {
        label: this.getDashboardBreadcrumbName(),
        link: this.getDashboardBreadcrumbPath(),
      },
    ]);
  }

  private updateActions() {
    this.$navbarMenu.setActions([
      {
        icon: 'bi-save',
        label: 'Save',
      },
      {
        icon: 'bi-x-square',
        label: 'Cancel',
      },
    ]);
  }

  getDashboardBreadcrumbName() {
    return 'System';
  }

  getDashboardBreadcrumbPath() {
    return '/monitored-systems/1';
  }
}
