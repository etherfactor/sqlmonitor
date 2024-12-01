import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ListComponent, TableColumn } from '../../../../shared/components/_base/list/list.component';
import { TableHeaderComponent } from '../../../../shared/components/table-header/table-header.component';
import { TableComponent } from '../../../../shared/components/table/table.component';
import { ScriptInterpreter } from '../../../../shared/models/script-interpreter';
import { BodyService } from '../../../../shared/services/body/body.service';
import { NavbarMenuAction, NavbarMenuBreadcrumb, NavbarMenuService } from '../../../../shared/services/navbar-menu/navbar-menu.service';
import { ScriptInterpreterService } from '../../../../shared/services/script-interpreter/script-interpreter.service';
import { Bound } from '../../../../shared/utilities/bound/bound.util';
import { EntitySet } from '../../../../shared/utilities/odata/odata.util';

@Component({
  selector: 'script-interpreter-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    TableComponent,
    TableHeaderComponent,
  ],
  templateUrl: './script-interpreter-list.component.html',
  styleUrl: './script-interpreter-list.component.scss'
})
export class ScriptInterpreterListComponent extends ListComponent<ScriptInterpreter> implements OnInit {
  
  private readonly $scriptInterpreter: ScriptInterpreterService;
  private readonly $router: Router;

  constructor(
    $body: BodyService,
    $form: FormBuilder,
    $modal: NgbModal,
    $scriptInterpreter: ScriptInterpreterService,
    $navbarMenu: NavbarMenuService,
    $router: Router,
  ) {
    super($body, $form, $modal, $navbarMenu);
    this.$scriptInterpreter = $scriptInterpreter;
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
        label: 'Script Interpreters',
        link: '/admin/script-interpreters',
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

  protected override getEntitySet(): EntitySet<ScriptInterpreter> {
    return this.$scriptInterpreter.set;
  }

  @Bound new() {
    this.$router.navigate(['/admin/script-interpreter', 'new']);
  }
}
