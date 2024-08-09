import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgSelectModule } from '@ng-select/ng-select';
import { DateTime } from 'luxon';
import { Observable } from 'rxjs';
import { EditableComponent } from '../../../../shared/components/_base/editable/editable.component';
import { InputLuxonDatetimeComponent } from '../../../../shared/components/input-luxon-datetime/input-luxon-datetime.component';
import { MonitoredResource, monitoredResourceForm } from '../../../../shared/models/monitored-resource';
import { BodyService } from '../../../../shared/services/body/body.service';
import { MonitoredResourceService } from '../../../../shared/services/monitored-resource/monitored-resource.service';
import { NavbarMenuAction, NavbarMenuBreadcrumb, NavbarMenuService } from '../../../../shared/services/navbar-menu/navbar-menu.service';
import { Guid, GuidZ } from '../../../../shared/types/guid/guid';

@Component({
  selector: 'app-monitored-resource-detail',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InputLuxonDatetimeComponent,
    NgSelectModule,
    ReactiveFormsModule,
  ],
  templateUrl: './monitored-resource-detail.component.html',
  styleUrl: './monitored-resource-detail.component.scss'
})
export class MonitoredResourceDetailComponent extends EditableComponent<MonitoredResource, Guid> {

  private readonly $form: FormBuilder;
  private readonly $monitoredSystem: MonitoredResourceService;

  constructor(
    $activatedRoute: ActivatedRoute,
    $body: BodyService,
    $form: FormBuilder,
    $monitoredSystem: MonitoredResourceService,
    $navbarMenu: NavbarMenuService,
    $router: Router,
  ) {
    super($activatedRoute, $body, $navbarMenu, $router, GuidZ);
    this.$form = $form;
    this.$monitoredSystem = $monitoredSystem;
  }

  protected override loadRecord(id: Guid) {
    return this.$monitoredSystem.get(id);
  }

  protected override createEmptyRecord(): MonitoredResource {
    return {
      createdAt: DateTime.now(),
      isActive: true,
    } as MonitoredResource;
  }

  protected override loadForm(record: MonitoredResource) {
    const form = monitoredResourceForm(this.$form, record);
    if (!record.id) {
      form.controls.isActive.markAsDirty();
    }
    return form;
  }

  protected override createRecord(record: Partial<MonitoredResource>): Observable<MonitoredResource> {
    return this.$monitoredSystem.create(record);
  }

  protected override updateRecord(id: Guid, record: Partial<MonitoredResource>): Observable<MonitoredResource> {
    return this.$monitoredSystem.update(id, record);
  }

  protected override navigateToRecord(record: MonitoredResource): void {
    this.$router.navigate(['/admin/resource', record.id]);
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
        label: 'Resources',
        link: '/admin/resources',
      },
    ];

    if (!this.isNew) {
      breadcrumbs.push({
        label: this.entity.name,
        link: `/admin/resource/${this.id}`,
      });
    } else {
      breadcrumbs.push({
        label: 'New Record',
        link: `/admin/resource/new`,
      });
    }

    return breadcrumbs;
  }
}
