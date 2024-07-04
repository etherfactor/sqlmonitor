import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { NgSelectModule } from '@ng-select/ng-select';
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
    $navbarMenu: NavbarMenuService,
    $form: FormBuilder,
    $monitoredSystem: MonitoredSystemService,
  ) {
    super($activatedRoute, $body, $navbarMenu, GuidZ);
    this.$form = $form;
    this.$monitoredSystem = $monitoredSystem;
  }

  protected loadRecord(id: Guid) {
    return this.$monitoredSystem.get(id);
  }

  protected loadForm(record: MonitoredSystem) {
    const form = monitoredSystemForm(this.$form, record);
    return form;
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
        label: 'Monitored Systems',
        link: '/monitored-systems',
      },
    ];

    if (!this.isNew) {
      breadcrumbs.push({
        label: this.entity.name,
        link: `/monitored-systems/${this.id}`,
      });
    } else {
      breadcrumbs.push({
        label: 'New Record',
        link: `/monitored-systems/new`,
      });
    }

    return breadcrumbs;
  }
}
