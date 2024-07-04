import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { NgSelectModule } from '@ng-select/ng-select';
import { MonitoredSystem, monitoredSystemForm } from '../../../../shared/models/monitored-system';
import { BodyContainerType, BodyService } from '../../../../shared/services/body/body.service';
import { MonitoredSystemService } from '../../../../shared/services/monitored-system/monitored-system.service';
import { NavbarMenuService } from '../../../../shared/services/navbar-menu/navbar-menu.service';
import { Guid } from '../../../../shared/types/guid/guid';
import { DefaultControlTypes, TypedFormGroup } from '../../../../shared/utilities/form/form.util';

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
  private readonly $monitoredSystem: MonitoredSystemService;
  private readonly $navbarMenu: NavbarMenuService;

  id?: Guid;
  isLoading: boolean = true;
  form?: TypedFormGroup<MonitoredSystem, DefaultControlTypes>;

  constructor(
    $activatedRoute: ActivatedRoute,
    $body: BodyService,
    $form: FormBuilder,
    $monitoredSystem: MonitoredSystemService,
    $navbarMenu: NavbarMenuService,
  ) {
    this.$activatedRoute = $activatedRoute;
    this.$body = $body;
    this.$form = $form;
    this.$monitoredSystem = $monitoredSystem;
    this.$navbarMenu = $navbarMenu;
  }

  ngOnInit(): void {
    this.$body.setContainer(BodyContainerType.Normal);
    this.updateBreadcrumbs();
    this.updateActions();

    this.id = this.$activatedRoute.snapshot.paramMap.get('id') as Guid;

    this.loadRecord(this.id);
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
    return `/monitored-systems/${this.id}`;
  }

  private loadRecord(id: Guid) {
    this.isLoading = true;

    this.$monitoredSystem.get(id).subscribe(record => {
      this.isLoading = false;
      this.loadForm(record);
    });
  }

  private loadForm(record: MonitoredSystem) {
    this.form = monitoredSystemForm(this.$form, record);
  }
}
