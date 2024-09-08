import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgSelectModule } from '@ng-select/ng-select';
import { DateTime } from 'luxon';
import { Observable } from 'rxjs';
import { EditableComponent } from '../../../../shared/components/_base/editable/editable.component';
import { InputLuxonDatetimeComponent } from '../../../../shared/components/input-luxon-datetime/input-luxon-datetime.component';
import { TableComponent } from '../../../../shared/components/table/table.component';
import { Group, groupForm } from '../../../../shared/models/group';
import { BodyService } from '../../../../shared/services/body/body.service';
import { GroupService } from '../../../../shared/services/group/group.service';
import { NavbarMenuAction, NavbarMenuBreadcrumb, NavbarMenuService } from '../../../../shared/services/navbar-menu/navbar-menu.service';
import { Guid, GuidZ } from '../../../../shared/types/guid/guid';

@Component({
  selector: 'group-detail',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InputLuxonDatetimeComponent,
    NgSelectModule,
    ReactiveFormsModule,
    TableComponent,
  ],
  templateUrl: './group-detail.component.html',
  styleUrl: './group-detail.component.scss'
})
export class GroupDetailComponent extends EditableComponent<Group, Guid> {

  private readonly $form: FormBuilder;
  private readonly $group: GroupService;
  
  constructor(
    $activatedRoute: ActivatedRoute,
    $body: BodyService,
    $form: FormBuilder,
    $user: GroupService,
    $navbarMenu: NavbarMenuService,
    $router: Router,
  ) {
    super($activatedRoute, $body, $navbarMenu, $router, GuidZ);
    this.$form = $form;
    this.$group = $user;
  }

  protected override loadRecord(id: Guid) {
    return this.$group.get(id);
  }

  protected override createEmptyRecord(): Group {
    return {
      createdAt: DateTime.now(),
    } as Group;
  }

  protected override loadForm(record: Group) {
    const form = groupForm(this.$form, record);
    return form;
  }

  protected override createRecord(record: Partial<Group>): Observable<Group> {
    return this.$group.create(record);
  }

  protected override updateRecord(id: Guid, record: Partial<Group>): Observable<Group> {
    return this.$group.update(id, record);
  }

  protected override navigateToRecord(record: Group): void {
    this.$router.navigate(['/admin/groups', record.id]);
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
        label: 'Groups',
        link: '/admin/groups',
      },
    ];

    if (!this.isNew) {
      breadcrumbs.push({
        label: this.entity.name,
        link: `/admin/group/${this.id}`,
      });
    } else {
      breadcrumbs.push({
        label: 'New Record',
        link: `/admin/group/new`,
      });
    }

    return breadcrumbs;
  }
}
