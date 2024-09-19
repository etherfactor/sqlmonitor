import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ListComponent, TableColumn } from '../../../../shared/components/_base/list/list.component';
import { TableHeaderComponent } from '../../../../shared/components/table-header/table-header.component';
import { TableComponent } from '../../../../shared/components/table/table.component';
import { Metric } from '../../../../shared/models/metric';
import { BodyService } from '../../../../shared/services/body/body.service';
import { MetricService } from '../../../../shared/services/metric/metric.service';
import { NavbarMenuAction, NavbarMenuBreadcrumb, NavbarMenuService } from '../../../../shared/services/navbar-menu/navbar-menu.service';
import { Bound } from '../../../../shared/utilities/bound/bound.util';
import { EntitySet } from '../../../../shared/utilities/odata/odata.util';

@Component({
  selector: 'metric-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    TableComponent,
    TableHeaderComponent,
  ],
  templateUrl: './metric-list.component.html',
  styleUrl: './metric-list.component.scss'
})
export class MetricListComponent extends ListComponent<Metric> implements OnInit {

  private readonly $metric: MetricService;
  private readonly $router: Router;

  constructor(
    $body: BodyService,
    $form: FormBuilder,
    $modal: NgbModal,
    $metric: MetricService,
    $navbarMenu: NavbarMenuService,
    $router: Router,
  ) {
    super($body, $form, $modal, $navbarMenu);
    this.$metric = $metric;
    this.$router = $router;
  }

  override ngOnInit() {
    this.onSortChange({ column: 'name', direction: 'asc' });
    super.ngOnInit();
  }

  override get perPage(): number {
    return 10;
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
        label: 'Administration',
        link: '/admin',
      },
      {
        label: 'Metrics',
        link: '/admin/metrics',
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

  protected override getEntitySet(): EntitySet<Metric> {
    return this.$metric.set;
  }

  @Bound new() {
    this.$router.navigate(['/admin/metric', 'new']);
  }
}
