import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormsModule, ReactiveFormsModule, ValidationErrors } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgSelectModule } from '@ng-select/ng-select';
import { DateTime } from 'luxon';
import { MonacoEditorModule } from 'ngx-monaco-editor-v2';
import { Observable } from 'rxjs';
import { EditableComponent } from '../../../../shared/components/_base/editable/editable.component';
import { InputLuxonDatetimeComponent } from '../../../../shared/components/input-luxon-datetime/input-luxon-datetime.component';
import { TableComponent } from '../../../../shared/components/table/table.component';
import { Script, scriptForm } from '../../../../shared/models/script';
import { ScriptMetric } from '../../../../shared/models/script-metric';
import { ScriptVariant, scriptVariantForm } from '../../../../shared/models/script-variant';
import { BodyService } from '../../../../shared/services/body/body.service';
import { NavbarMenuAction, NavbarMenuBreadcrumb, NavbarMenuService } from '../../../../shared/services/navbar-menu/navbar-menu.service';
import { ScriptService } from '../../../../shared/services/script/script.service';
import { Guid, GuidZ } from '../../../../shared/types/guid/guid';

@Component({
  selector: 'script-detail',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InputLuxonDatetimeComponent,
    MonacoEditorModule,
    NgSelectModule,
    ReactiveFormsModule,
    TableComponent,
  ],
  templateUrl: './script-detail.component.html',
  styleUrl: './script-detail.component.scss'
})
export class ScriptDetailComponent extends EditableComponent<Script, Guid> implements OnInit {

  private readonly $form: FormBuilder;
  private readonly $user: ScriptService;

  newScriptInterpreterIdFormControl = new FormControl<number | undefined>(undefined, {
    nonNullable: true,
    validators: [
      control => this.isNotDuplicateType(control.value),
    ],
  });

  constructor(
    $activatedRoute: ActivatedRoute,
    $body: BodyService,
    $form: FormBuilder,
    $script: ScriptService,
    $navbarMenu: NavbarMenuService,
    $router: Router,
  ) {
    super($activatedRoute, $body, $navbarMenu, $router, GuidZ);
    this.$form = $form;
    this.$user = $script;
  }

  protected override loadRecord(id: Guid) {
    return this.$user.get(id);
  }

  protected override createEmptyRecord(): Script {
    return {
      createdAt: DateTime.now(),
      isActive: true,
      variants: [] as ScriptVariant[],
      metrics: [{}] as ScriptMetric[],
    } as Script;
  }

  protected override loadForm(record: Script) {
    const form = scriptForm(this.$form, record);
    if (!record.id) {
      form.controls.isActive.markAsDirty();
    }
    return form;
  }

  protected override createRecord(record: Partial<Script>): Observable<Script> {
    return this.$user.create(record);
  }

  protected override updateRecord(id: Guid, record: Partial<Script>): Observable<Script> {
    return this.$user.update(id, record);
  }

  protected override navigateToRecord(record: Script): void {
    this.$router.navigate(['/admin/scripts', record.id]);
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
        label: 'Scripts',
        link: '/admin/scripts',
      },
    ];

    if (!this.isNew) {
      breadcrumbs.push({
        label: this.entity.name,
        link: `/admin/script/${this.id}`,
      });
    } else {
      breadcrumbs.push({
        label: 'New Record',
        link: `/admin/script/new`,
      });
    }

    return breadcrumbs;
  }

  private isNotDuplicateType(value: number): ValidationErrors | null {
    const currentVariants = this.form?.value?.variants ?? [];
    const currentTypes = currentVariants.map(item => item.scriptInterpreterId);

    if (currentTypes.indexOf(value) >= 0) {
      return { self: 'Duplicate script interpreter' };
    }

    return null;
  }

  tryAddVariant() {
    this.newScriptInterpreterIdFormControl.markAsTouched();
    if (this.newScriptInterpreterIdFormControl.invalid)
      return;

    if (!this.form)
      return;

    const newScriptInterpreterId = this.newScriptInterpreterIdFormControl.value!;
    const newForm = scriptVariantForm(this.$form, { scriptInterpreterId: newScriptInterpreterId } as ScriptVariant);
    newForm.controls.scriptInterpreterId.markAllAsTouched();

    let useIndex = 0;
    while ((this.form.controls.variants.controls[useIndex]?.value?.scriptInterpreterId ?? -1) < (newScriptInterpreterId ?? 1)) {
      useIndex++;
    }
    this.form.controls.variants.insert(useIndex, newForm);

    this.newScriptInterpreterIdFormControl.markAsUntouched();
    this.newScriptInterpreterIdFormControl.updateValueAndValidity();
  }

  removeVariant(index: number) {
    if (!this.form)
      return;

    this.form.controls.variants.removeAt(index);
  }

  getText(index: number) {
    if (!this.form)
      return '';

    return this.form.controls.variants.controls[index]?.value?.scriptText ?? '';
  }

  setText(index: number, value: string | undefined) {
    if (!this.form)
      return undefined;

    return this.form.controls.variants.controls[index]?.controls?.scriptText?.setValue(value);
  }

  getScriptInterpreterLabel(scriptInterpreterId: number) {
    switch (scriptInterpreterId) {
      case 1:
        return 'PowerShell 7';

      case 2:
        return 'PowerShell 5';

      case 3:
        return 'Bash';

      default:
        return 'Unknown';
    }
  }
}
