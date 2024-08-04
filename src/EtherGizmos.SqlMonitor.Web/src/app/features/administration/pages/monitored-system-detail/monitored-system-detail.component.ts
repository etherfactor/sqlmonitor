import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgSelectModule } from '@ng-select/ng-select';
import { DateTime } from 'luxon';
import { Observable } from 'rxjs';
import { EditableComponent } from '../../../../shared/components/_base/editable/editable.component';
import { InputLuxonDatetimeComponent } from '../../../../shared/components/input-luxon-datetime/input-luxon-datetime.component';
import { MonitoredSystem, monitoredSystemForm } from '../../../../shared/models/monitored-system';
import { BodyService } from '../../../../shared/services/body/body.service';
import { MonitoredSystemService } from '../../../../shared/services/monitored-system/monitored-system.service';
import { NavbarMenuAction, NavbarMenuBreadcrumb, NavbarMenuService } from '../../../../shared/services/navbar-menu/navbar-menu.service';
import { Guid, GuidZ } from '../../../../shared/types/guid/guid';

@Component({
  selector: 'app-monitored-system-detail',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InputLuxonDatetimeComponent,
    NgSelectModule,
    ReactiveFormsModule,
  ],
  templateUrl: './monitored-system-detail.component.html',
  styleUrl: './monitored-system-detail.component.scss'
})
export class MonitoredSystemDetailComponent extends EditableComponent<MonitoredSystem, Guid> implements OnInit {

  private readonly $form: FormBuilder;
  private readonly $monitoredSystem: MonitoredSystemService;

  constructor(
    $activatedRoute: ActivatedRoute,
    $body: BodyService,
    $form: FormBuilder,
    $monitoredSystem: MonitoredSystemService,
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

  protected override createEmptyRecord(): MonitoredSystem {
    return {
      createdAt: DateTime.now(),
      isActive: true,
    } as MonitoredSystem;
  }

  protected override loadForm(record: MonitoredSystem) {
    const form = monitoredSystemForm(this.$form, record);
    if (!record.id) {
      form.controls.isActive.markAsDirty();
    }
    return form;
  }

  protected override createRecord(record: Partial<MonitoredSystem>): Observable<MonitoredSystem> {
    return this.$monitoredSystem.create(record);
  }

  protected override updateRecord(id: Guid, record: Partial<MonitoredSystem>): Observable<MonitoredSystem> {
    return this.$monitoredSystem.update(id, record);
  }

  protected override navigateToRecord(record: MonitoredSystem): void {
    this.$router.navigate(['/monitored-systems', record.id]);
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
        label: 'Systems',
        link: '/admin/systems',
      },
    ];

    if (!this.isNew) {
      breadcrumbs.push({
        label: this.entity.name,
        link: `/admin/system/${this.id}`,
      });
    } else {
      breadcrumbs.push({
        label: 'New Record',
        link: `/admin/system/new`,
      });
    }

    return breadcrumbs;
  }
}
