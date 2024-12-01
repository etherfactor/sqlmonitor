import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgSelectModule } from '@ng-select/ng-select';
import { DateTime } from 'luxon';
import { Observable } from 'rxjs';
import { z } from 'zod';
import { EditableComponent } from '../../../../shared/components/_base/editable/editable.component';
import { InputLuxonDatetimeComponent } from '../../../../shared/components/input-luxon-datetime/input-luxon-datetime.component';
import { ScriptInterpreter, scriptInterpreterForm } from '../../../../shared/models/script-interpreter';
import { BodyService } from '../../../../shared/services/body/body.service';
import { NavbarMenuAction, NavbarMenuBreadcrumb, NavbarMenuService } from '../../../../shared/services/navbar-menu/navbar-menu.service';
import { ScriptInterpreterService } from '../../../../shared/services/script-interpreter/script-interpreter.service';

const StringToIntZ = z.string().refine(s => {
  try {
    parseInt(s);
    return true;
  } catch {
    return false;
  }
}).transform(s => {
  return parseInt(s);
});

@Component({
  selector: 'script-interpreter-detail',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InputLuxonDatetimeComponent,
    NgSelectModule,
    ReactiveFormsModule,
  ],
  templateUrl: './script-interpreter-detail.component.html',
  styleUrl: './script-interpreter-detail.component.scss'
})
export class ScriptInterpreterDetailComponent extends EditableComponent<ScriptInterpreter, number> implements OnInit {

  private readonly $form: FormBuilder;
  private readonly $scriptInterpreter: ScriptInterpreterService;

  constructor(
    $activatedRoute: ActivatedRoute,
    $body: BodyService,
    $form: FormBuilder,
    $scriptInterpreter: ScriptInterpreterService,
    $navbarMenu: NavbarMenuService,
    $router: Router,
  ) {
    super($activatedRoute, $body, $navbarMenu, $router, StringToIntZ);
    this.$form = $form;
    this.$scriptInterpreter = $scriptInterpreter;
  }

  protected override loadRecord(id: number) {
    return this.$scriptInterpreter.get(id);
  }

  protected override createEmptyRecord(): ScriptInterpreter {
    return {
      createdAt: DateTime.now(),
    } as ScriptInterpreter;
  }

  protected override loadForm(record: ScriptInterpreter) {
    const form = scriptInterpreterForm(this.$form, record);
    return form;
  }

  protected override createRecord(record: Partial<ScriptInterpreter>): Observable<ScriptInterpreter> {
    return this.$scriptInterpreter.create(record);
  }

  protected override updateRecord(id: number, record: Partial<ScriptInterpreter>): Observable<ScriptInterpreter> {
    return this.$scriptInterpreter.update(id, record);
  }

  protected override navigateToRecord(record: ScriptInterpreter): void {
    this.$router.navigate(['/admin/script-interpreter', record.id]);
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
        label: 'Script Interpreters',
        link: '/admin/script-interpreters',
      },
    ];

    if (!this.isNew) {
      breadcrumbs.push({
        label: this.entity.name,
        link: `/admin/script-interpreter/${this.id}`,
      });
    } else {
      breadcrumbs.push({
        label: 'New Record',
        link: `/admin/script-interpreter/new`,
      });
    }

    return breadcrumbs;
  }
}
