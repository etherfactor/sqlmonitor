import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ListComponent, TableColumn } from '../../../../shared/components/_base/list/list.component';
import { FilterBuilderModalComponent } from '../../../../shared/components/filter-builder-modal/filter-builder-modal.component';
import { TableHeaderComponent } from '../../../../shared/components/table-header/table-header.component';
import { TableComponent } from '../../../../shared/components/table/table.component';
import { MonitoredSystem } from '../../../../shared/models/monitored-system';
import { BodyService } from '../../../../shared/services/body/body.service';
import { MonitoredSystemService } from '../../../../shared/services/monitored-system/monitored-system.service';
import { NavbarMenuAction, NavbarMenuBreadcrumb, NavbarMenuService } from '../../../../shared/services/navbar-menu/navbar-menu.service';
import { Bound } from '../../../../shared/utilities/bound/bound.util';
import { EntitySet } from '../../../../shared/utilities/odata/odata.util';

@Component({
  selector: 'app-monitored-system-list',
  standalone: true,
  imports: [
    CommonModule,
    FilterBuilderModalComponent,
    RouterModule,
    TableComponent,
    TableHeaderComponent,
  ],
  templateUrl: './monitored-system-list.component.html',
  styleUrl: './monitored-system-list.component.scss'
})
export class MonitoredSystemListComponent extends ListComponent<MonitoredSystem> implements OnInit {

  private readonly $monitoredSystem: MonitoredSystemService;
  private readonly $router: Router;

  //records: MonitoredSystem[] = [
  //  {
  //    id: 'fca5315f-6e2f-4a78-baac-bdb061e6d8fc' as Guid,
  //    createdAt: DateTime.now(),
  //    createdByUserId: generateGuid(),
  //    modifiedAt: undefined,
  //    modifiedByUserId: undefined,
  //    name: 'Example System',
  //    description: 'Desc',
  //    isActive: true,
  //  }
  //];

  constructor(
    $body: BodyService,
    $form: FormBuilder,
    $modal: NgbModal,
    $monitoredSystem: MonitoredSystemService,
    $navbarMenu: NavbarMenuService,
    $router: Router,
  ) {
    super($body, $form, $modal, $navbarMenu);
    this.$monitoredSystem = $monitoredSystem;
    this.$router = $router;
  }

  override ngOnInit() {
    super.ngOnInit();
    this.onSortChange({ column: 'name', direction: 'asc' });
  }

  protected override get actions(): NavbarMenuAction[] {
    const actions: NavbarMenuAction[] = [
      {
        icon: 'bi-layout-three-columns',
        label: 'Edit Columns',
      },
      {
        icon: 'bi-plus-square',
        label: 'Add',
        callback: this.new,
      },
    ];

    return actions;
  }

  protected override get breadcrumbs(): NavbarMenuBreadcrumb[] {
    const breadcrumbs: NavbarMenuBreadcrumb[] = [
      {
        label: 'Home',
        link: '/',
      },
      {
        label: 'Monitored Systems',
        link: '/monitored-systems',
      },
    ];

    return breadcrumbs;
  }

  protected override get columns() {
    const columns: TableColumn[] = [
      { name: 'id', displayName: 'Id', type: 'guid' },
      { name: 'name', displayName: 'Name', type: 'string' },
      { name: 'description', displayName: 'Description', type: 'string' },
      { name: 'isActive', displayName: 'Is active', type: 'boolean' },
      { name: 'createdAt', displayName: 'Created at', type: 'datetime' },
      { name: 'createdByUserId', displayName: 'Created by user id', type: 'guid' },
      { name: 'modifiedAt', displayName: 'Modified at', type: 'datetime' },
      { name: 'modifiedByUserId', displayName: 'Modified by user id', type: 'guid' },
    ];

    return columns;
  }

  protected override getEntitySet(): EntitySet<MonitoredSystem> {
    return this.$monitoredSystem.set;
  }

  @Bound new() {
    this.$router.navigate(['/monitored-system/new']);
  }
}
