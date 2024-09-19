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
import { Metric, metricForm } from '../../../../shared/models/metric';
import { BodyService } from '../../../../shared/services/body/body.service';
import { MetricService } from '../../../../shared/services/metric/metric.service';
import { NavbarMenuAction, NavbarMenuBreadcrumb, NavbarMenuService } from '../../../../shared/services/navbar-menu/navbar-menu.service';

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
  selector: 'metric-detail',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InputLuxonDatetimeComponent,
    NgSelectModule,
    ReactiveFormsModule,
  ],
  templateUrl: './metric-detail.component.html',
  styleUrl: './metric-detail.component.scss'
})
export class MetricDetailComponent extends EditableComponent<Metric, number> implements OnInit {

  private readonly $form: FormBuilder;
  private readonly $metric: MetricService;

  constructor(
    $activatedRoute: ActivatedRoute,
    $body: BodyService,
    $form: FormBuilder,
    $metric: MetricService,
    $navbarMenu: NavbarMenuService,
    $router: Router,
  ) {
    super($activatedRoute, $body, $navbarMenu, $router, StringToIntZ);
    this.$form = $form;
    this.$metric = $metric;
  }

  protected override loadRecord(id: number) {
    return this.$metric.get(id);
  }

  protected override createEmptyRecord(): Metric {
    return {
      createdAt: DateTime.now(),
      isActive: true,
    } as Metric;
  }

  protected override loadForm(record: Metric) {
    const form = metricForm(this.$form, record);
    if (!record.id) {
      form.controls.isActive.markAsDirty();
    }
    return form;
  }

  protected override createRecord(record: Partial<Metric>): Observable<Metric> {
    return this.$metric.create(record);
  }

  protected override updateRecord(id: number, record: Partial<Metric>): Observable<Metric> {
    return this.$metric.update(id, record);
  }

  protected override navigateToRecord(record: Metric): void {
    this.$router.navigate(['/admin/metric', record.id]);
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
        label: 'Metrics',
        link: '/admin/metrics',
      },
    ];

    if (!this.isNew) {
      breadcrumbs.push({
        label: this.entity.name,
        link: `/admin/metric/${this.id}`,
      });
    } else {
      breadcrumbs.push({
        label: 'New Record',
        link: `/admin/metric/new`,
      });
    }

    return breadcrumbs;
  }
}
