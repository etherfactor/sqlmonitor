import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgSelectModule } from '@ng-select/ng-select';
import { DateTime } from 'luxon';
import { Observable } from 'rxjs';
import { EditableComponent } from '../../../../shared/components/_base/editable/editable.component';
import { InputLuxonDatetimeComponent } from '../../../../shared/components/input-luxon-datetime/input-luxon-datetime.component';
import { MonitoredEnvironment, monitoredEnvironmentForm } from '../../../../shared/models/monitored-environment';
import { BodyService } from '../../../../shared/services/body/body.service';
import { MonitoredEnvironmentService } from '../../../../shared/services/monitored-environment/monitored-environment.service';
import { NavbarMenuAction, NavbarMenuBreadcrumb, NavbarMenuService } from '../../../../shared/services/navbar-menu/navbar-menu.service';
import { Guid, GuidZ } from '../../../../shared/types/guid/guid';

@Component({
  selector: 'app-monitored-environment-detail',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InputLuxonDatetimeComponent,
    NgSelectModule,
    ReactiveFormsModule,
  ],
  templateUrl: './monitored-environment-detail.component.html',
  styleUrl: './monitored-environment-detail.component.scss'
})
export class MonitoredEnvironmentDetailComponent extends EditableComponent<MonitoredEnvironment, Guid> implements OnInit {

  private readonly $form: FormBuilder;
  private readonly $MonitoredEnvironment: MonitoredEnvironmentService;

  constructor(
    $activatedRoute: ActivatedRoute,
    $body: BodyService,
    $form: FormBuilder,
    $MonitoredEnvironment: MonitoredEnvironmentService,
    $navbarMenu: NavbarMenuService,
    $router: Router,
  ) {
    super($activatedRoute, $body, $navbarMenu, $router, GuidZ);
    this.$form = $form;
    this.$MonitoredEnvironment = $MonitoredEnvironment;
  }

  protected override loadRecord(id: Guid) {
    return this.$MonitoredEnvironment.get(id);
  }

  protected override createEmptyRecord(): MonitoredEnvironment {
    return {
      createdAt: DateTime.now(),
      isActive: true,
    } as MonitoredEnvironment;
  }

  protected override loadForm(record: MonitoredEnvironment) {
    const form = monitoredEnvironmentForm(this.$form, record);
    if (!record.id) {
      form.controls.isActive.markAsDirty();
    }
    return form;
  }

  protected override createRecord(record: Partial<MonitoredEnvironment>): Observable<MonitoredEnvironment> {
    return this.$MonitoredEnvironment.create(record);
  }

  protected override updateRecord(id: Guid, record: Partial<MonitoredEnvironment>): Observable<MonitoredEnvironment> {
    return this.$MonitoredEnvironment.update(id, record);
  }

  protected override navigateToRecord(record: MonitoredEnvironment): void {
    this.$router.navigate(['/admin/environments', record.id]);
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
        label: 'Environments',
        link: '/admin/environments',
      },
    ];

    if (!this.isNew) {
      breadcrumbs.push({
        label: this.entity.name,
        link: `/admin/environment/${this.id}`,
      });
    } else {
      breadcrumbs.push({
        label: 'New Record',
        link: `/admin/environment/new`,
      });
    }

    return breadcrumbs;
  }
}
