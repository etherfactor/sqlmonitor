import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DateTime } from 'luxon';
import { FilterBuilderModalComponent, FilterGroup, FilterProperty, filterGroupForm } from '../../../../shared/components/filter-builder-modal/filter-builder-modal.component';
import { TableHeaderComponent } from '../../../../shared/components/table-header/table-header.component';
import { TableComponent } from '../../../../shared/components/table/table.component';
import { MonitoredSystem } from '../../../../shared/models/monitored-system';
import { BodyContainerType, BodyService } from '../../../../shared/services/body/body.service';
import { NavbarMenuService } from '../../../../shared/services/navbar-menu/navbar-menu.service';
import { Guid, generateGuid } from '../../../../shared/types/guid/guid';
import { FilterColumnCondition } from '../../../../shared/utilities/filter/filter.util';
import { DefaultControlTypes, TypedFormGroup } from '../../../../shared/utilities/form/form.util';

@Component({
  selector: 'app-monitored-system-list',
  standalone: true,
  imports: [
    CommonModule,
    FilterBuilderModalComponent,
    RouterModule,
    TableComponent,
    TableHeaderComponent,
  ],
  templateUrl: './monitored-system-list.component.html',
  styleUrl: './monitored-system-list.component.scss'
})
export class MonitoredSystemListComponent implements OnInit {

  private readonly $body: BodyService;
  private readonly $form: FormBuilder;
  private readonly $modal: NgbModal;
  private readonly $navbarMenu: NavbarMenuService;

  filterForm: TypedFormGroup<FilterGroup, DefaultControlTypes>;

  filter: FilterGroup;
  properties: FilterProperty[];

  records: MonitoredSystem[] = [
    {
      id: 'fca5315f-6e2f-4a78-baac-bdb061e6d8fc' as Guid,
      createdAt: DateTime.now(),
      createdByUserId: generateGuid(),
      modifiedAt: undefined,
      modifiedByUserId: undefined,
      name: 'Example System',
      description: 'Desc',
      isActive: true,
    }
  ];

  constructor(
    $body: BodyService,
    $form: FormBuilder,
    $modal: NgbModal,
    $navbarMenu: NavbarMenuService,
  ) {
    this.$body = $body;
    this.$form = $form;
    this.$modal = $modal;
    this.$navbarMenu = $navbarMenu;

    this.filter = {
      operator: 'and',
      conditions: [
        { property: undefined!, operator: undefined!, value: undefined! },
      ]
    };

    this.properties = [
      { name: 'id', displayName: 'Id', type: 'guid' },
      { name: 'name', displayName: 'Name', type: 'string' },
      { name: 'description', displayName: 'Description', type: 'string' },
      { name: 'isActive', displayName: 'Is active', type: 'boolean' },
      { name: 'createdAt', displayName: 'Created at', type: 'datetime' },
      { name: 'createdByUserId', displayName: 'Created by user id', type: 'guid' },
      { name: 'modifiedAt', displayName: 'Modified at', type: 'datetime' },
      { name: 'modifiedByUserId', displayName: 'Modified by user id', type: 'guid' },
    ];

    this.filterForm = filterGroupForm(this.$form, this.filter);
  }

  ngOnInit(): void {
    this.$body.setContainer(BodyContainerType.Normal);
    this.updateBreadcrumbs();
    this.updateActions();
  }

  openFilters() {
    const modalInstance = this.$modal.open(FilterBuilderModalComponent, { size: 'lg', centered: true, backdrop: 'static', keyboard: false });

    const component = modalInstance.componentInstance as FilterBuilderModalComponent;
    component.setFilter(this.filter, this.properties);

    modalInstance.result.then(
      (result: FilterGroup) => {
        console.log(result);
        this.filter = result;
      },
      dismissed => { }
    );
  }

  private updateBreadcrumbs() {
    this.$navbarMenu.setBreadcrumbs([
      {
        label: 'Home',
        link: '/',
      },
      {
        label: 'Monitored Systems',
        link: '/monitored-systems',
      },
    ]);
  }

  private updateActions() {
    this.$navbarMenu.setActions([
      {
        icon: 'bi-plus-square',
        label: 'Add',
      },
    ]);
  }

  onFilterChange(filters: FilterColumnCondition[]) {
    console.log(filters);
  }
}
