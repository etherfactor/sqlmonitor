import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgSelectModule } from '@ng-select/ng-select';
import { DateTime } from 'luxon';
import { Observable } from 'rxjs';
import { EditableComponent } from '../../../../shared/components/_base/editable/editable.component';
import { InputLuxonDatetimeComponent } from '../../../../shared/components/input-luxon-datetime/input-luxon-datetime.component';
import { TableComponent } from '../../../../shared/components/table/table.component';
import { Query, queryForm } from '../../../../shared/models/query';
import { QueryMetric } from '../../../../shared/models/query-metric';
import { QueryVariant } from '../../../../shared/models/query-variant';
import { getSqlTypeLabel } from '../../../../shared/models/sql-type';
import { BodyService } from '../../../../shared/services/body/body.service';
import { NavbarMenuAction, NavbarMenuBreadcrumb, NavbarMenuService } from '../../../../shared/services/navbar-menu/navbar-menu.service';
import { QueryService } from '../../../../shared/services/query/query.service';
import { Guid, GuidZ } from '../../../../shared/types/guid/guid';

@Component({
  selector: 'query-detail',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InputLuxonDatetimeComponent,
    NgSelectModule,
    ReactiveFormsModule,
    TableComponent,
  ],
  templateUrl: './query-detail.component.html',
  styleUrl: './query-detail.component.scss'
})
export class QueryDetailComponent extends EditableComponent<Query, Guid> implements OnInit {

  private readonly $form: FormBuilder;
  private readonly $user: QueryService;
  
  constructor(
    $activatedRoute: ActivatedRoute,
    $body: BodyService,
    $form: FormBuilder,
    $user: QueryService,
    $navbarMenu: NavbarMenuService,
    $router: Router,
  ) {
    super($activatedRoute, $body, $navbarMenu, $router, GuidZ);
    this.$form = $form;
    this.$user = $user;
  }

  protected override loadRecord(id: Guid) {
    return this.$user.get(id);
  }

  protected override createEmptyRecord(): Query {
    return {
      createdAt: DateTime.now(),
      isActive: true,
      variants: [{}] as QueryVariant[],
      metrics: [{}] as QueryMetric[],
    } as Query;
  }

  protected override loadForm(record: Query) {
    const form = queryForm(this.$form, record);
    if (!record.id) {
      form.controls.isActive.markAsDirty();
    }
    return form;
  }

  protected override createRecord(record: Partial<Query>): Observable<Query> {
    return this.$user.create(record);
  }

  protected override updateRecord(id: Guid, record: Partial<Query>): Observable<Query> {
    return this.$user.update(id, record);
  }

  protected override navigateToRecord(record: Query): void {
    this.$router.navigate(['/admin/queries', record.id]);
  }

  override get actions(): NavbarMenuAction[] {
    const actions: NavbarMenuAction[] = [];

    if (this.isEditing) {
      actions.push({
        icon: 'bi-save',
        label: 'Save',
        callback: this.save,
      });
      actions.push({
        icon: 'bi-x-square',
        label: 'Cancel',
        callback: this.cancel,
      });
    } else {
      actions.push({
        icon: 'bi-pencil',
        label: 'Edit',
        callback: this.edit,
      });
      actions.push({
        icon: 'bi-trash',
        label: 'Delete',
      });
    }

    return actions;
  }

  override get breadcrumbs(): NavbarMenuBreadcrumb[] {
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

    if (!this.isNew) {
      breadcrumbs.push({
        label: this.entity.name,
        link: `/admin/query/${this.id}`,
      });
    } else {
      breadcrumbs.push({
        label: 'New Record',
        link: `/admin/query/new`,
      });
    }

    return breadcrumbs;
  }

  getSqlTypeLabel = getSqlTypeLabel;
}
