import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ListComponent, TableColumn } from '../../../../shared/components/_base/list/list.component';
import { TableHeaderComponent } from '../../../../shared/components/table-header/table-header.component';
import { TableComponent } from '../../../../shared/components/table/table.component';
import { Query } from '../../../../shared/models/query';
import { BodyService } from '../../../../shared/services/body/body.service';
import { NavbarMenuAction, NavbarMenuBreadcrumb, NavbarMenuService } from '../../../../shared/services/navbar-menu/navbar-menu.service';
import { QueryService } from '../../../../shared/services/query/query.service';
import { Bound } from '../../../../shared/utilities/bound/bound.util';
import { EntitySet } from '../../../../shared/utilities/odata/odata.util';

@Component({
  selector: 'query-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    TableComponent,
    TableHeaderComponent,
  ],
  templateUrl: './query-list.component.html',
  styleUrl: './query-list.component.scss'
})
export class QueryListComponent extends ListComponent<Query> implements OnInit {

  private readonly $query: QueryService;
  private readonly $router: Router;

  constructor(
    $body: BodyService,
    $form: FormBuilder,
    $modal: NgbModal,
    $query: QueryService,
    $navbarMenu: NavbarMenuService,
    $router: Router,
  ) {
    super($body, $form, $modal, $navbarMenu);
    this.$query = $query;
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
        label: 'Queries',
        link: '/admin/queries',
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
      { name: 'createdByUserId', displayName: 'Created by query id', type: 'guid' },
      { name: 'modifiedAt', displayName: 'Modified at', type: 'datetime' },
      { name: 'modifiedByUserId', displayName: 'Modified by query id', type: 'guid' },
    ];

    return columns;
  }

  protected override getEntitySet(): EntitySet<Query> {
    return this.$query.set;
  }

  @Bound new() {
    this.$router.navigate(['/admin/query', 'new']);
  }
}
