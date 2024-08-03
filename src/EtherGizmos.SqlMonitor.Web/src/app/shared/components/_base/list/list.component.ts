import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DateTime } from 'luxon';
import { isEqual } from 'moderndash';
import { Subject, debounceTime, filter } from 'rxjs';
import { BodyContainerType, BodyService } from '../../../services/body/body.service';
import { NavbarMenuAction, NavbarMenuBreadcrumb, NavbarMenuService } from '../../../services/navbar-menu/navbar-menu.service';
import { Guid } from '../../../types/guid/guid';
import { FilterColumnCondition, FilterType } from '../../../utilities/filter/filter.util';
import { EntitySet, Value, o } from '../../../utilities/odata/odata.util';
import { SortColumn } from '../../../utilities/sort/sort.util';

@Component({
  selector: 'app-list',
  template: ''
})
export abstract class ListComponent<TEntity> implements OnInit {

  protected readonly $body: BodyService;
  protected readonly $form: FormBuilder;
  protected readonly $modal: NgbModal;
  protected readonly $navbarMenu: NavbarMenuService;

  private activeSort?: SortColumn;
  private activeFilters: FilterColumnCondition[] = [];

  private searchSubject = new Subject<void>();

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
  }

  ngOnInit(): void {
    this.$body.setContainer(BodyContainerType.Normal);

    this.initialize();

    let currentSort = this.activeSort;
    let currentFilters = this.activeFilters;
    this.searchSubject.pipe(
      filter(() => {
        const sortEqual = isEqual(this.activeSort, currentSort);
        const filtersEqual = isEqual(this.activeFilters, currentFilters);
        if (sortEqual && filtersEqual)
          return false;

        currentSort = this.activeSort;
        currentFilters = this.activeFilters;

        return true;
      }),
      debounceTime(250),
    ).subscribe(() => {
      this.search();
    });
  }

  private initialize() {
    this._activeColumns = [...this.columns];
    this.refresh();
    this.search();
  }

  private refresh() {
    this.updateActions();
    this.updateBreadcrumbs();
  }

  private search() {
    let set = this.getEntitySet();
    for (const filter of this.activeFilters) {
      let rightValue: Value<any>;

      if (filter.value) {
        switch (filter.type) {
          case 'boolean':
            rightValue = o.bool(filter.value as boolean);
            break;

          case 'datetime':
            rightValue = o.dateTime(filter.value as DateTime);
            break;

          case 'guid':
            rightValue = o.guid(filter.value as Guid);
            break;

          case 'number':
            rightValue = o.int(filter.value as number);
            break;

          case 'string':
            rightValue = o.string(filter.value as string);
            break;

          default:
            throw new Error('Not implemented type');
        }
      }

      switch (filter.operator) {
        case 'contains':
          if (filter.type !== 'string')
            throw new Error(`Unable to filter on ${filter.column} of type ${filter.type}; requires ${'string'}`);
          set = set.filter(b =>
            o.contains(
              b.prop(filter.column as keyof TEntity & string) as Value<string>,
              rightValue,
            ),
          );
          break;

        case 'ends_with':
          if (filter.type !== 'string')
            throw new Error(`Unable to filter on ${filter.column} of type ${filter.type}; requires ${'string'}`);
          set = set.filter(b =>
            o.endsWith(
              b.prop(filter.column as keyof TEntity & string) as Value<string>,
              rightValue,
            ),
          );
          break;

        case 'equals':
          set = set.filter(b =>
            o.eq(
              b.prop(filter.column as keyof TEntity & string),
              rightValue,
            ),
          );
          break;

        case 'false':
          if (filter.type !== 'boolean')
            throw new Error(`Unable to filter on ${filter.column} of type ${filter.type}; requires ${'boolean'}`);
          set = set.filter(b =>
            o.eq(
              b.prop(filter.column as keyof TEntity & string) as Value<boolean>,
              o.bool(false),
            ),
          );
          break;

        case 'greater':
          set = set.filter(b =>
            o.gt(
              b.prop(filter.column as keyof TEntity & string),
              rightValue,
            ),
          );
          break;

        case 'greater_equals':
          set = set.filter(b =>
            o.ge(
              b.prop(filter.column as keyof TEntity & string),
              rightValue,
            ),
          );
          break;

        case 'less':
          set = set.filter(b =>
            o.lt(
              b.prop(filter.column as keyof TEntity & string),
              rightValue,
            ),
          );
          break;

        case 'less_equals':
          set = set.filter(b =>
            o.le(
              b.prop(filter.column as keyof TEntity & string),
              rightValue,
            ),
          );
          break;

        case 'not_contains':
          if (filter.type !== 'string')
            throw new Error(`Unable to filter on ${filter.column} of type ${filter.type}; requires ${'string'}`);
          set = set.filter(b =>
            o.not(
              o.contains(
                b.prop(filter.column as keyof TEntity & string) as Value<string>,
                rightValue,
              ),
            ),
          );
          break;

        case 'not_equals':
          set = set.filter(b =>
            o.ne(
              b.prop(filter.column as keyof TEntity & string),
              rightValue,
            ),
          );
          break;

        case 'starts_with':
          if (filter.type !== 'string')
            throw new Error(`Unable to filter on ${filter.column} of type ${filter.type}; requires ${'string'}`);
          set = set.filter(b =>
            o.startsWith(
              b.prop(filter.column as keyof TEntity & string) as Value<string>,
              rightValue,
            ),
          );
          break;

        case 'true':
          if (filter.type !== 'boolean')
            throw new Error(`Unable to filter on ${filter.column} of type ${filter.type}; requires ${'boolean'}`);
          set = set.filter(b =>
            o.eq(
              b.prop(filter.column as keyof TEntity & string) as Value<boolean>,
              o.bool(true),
            ),
          );
          break;
      }
    }

    if (this.activeSort) {
      set = set.orderBy(this.activeSort.column as keyof TEntity & string, this.activeSort.direction);
    }

    console.log(set.getParams());
  }

  protected abstract get actions(): NavbarMenuAction[];

  private updateActions() {
    this.$navbarMenu.setActions(this.actions);
  }

  protected abstract get breadcrumbs(): NavbarMenuBreadcrumb[];

  private updateBreadcrumbs() {
    this.$navbarMenu.setBreadcrumbs(this.breadcrumbs);
  }

  protected abstract get columns(): TableColumn[];

  private _activeColumns: TableColumn[] = [];
  protected get activeColumns(): TableColumn[] {
    return this._activeColumns;
  }

  protected abstract getEntitySet(): EntitySet<TEntity>;

  onSortChange(sort: SortColumn) {
    this.activeSort = sort;
    this.searchSubject.next();
  }

  onFilterChange(filters: FilterColumnCondition[]) {
    this.activeFilters = filters;
    this.searchSubject.next();
  }
}

export interface TableColumn {
  name: string;
  displayName: string;
  type: FilterType;
}
