import { CommonModule } from '@angular/common';
import { Component, ContentChild, EventEmitter, Input, Output, TemplateRef } from '@angular/core';
import { isEqual } from 'moderndash';
import { Observable, Subscription, combineLatest, debounceTime, distinctUntilChanged, map, startWith } from 'rxjs';
import { SortTableService } from '../../services/sort-table/sort-table.service';
import { generateGuid } from '../../types/guid/guid';
import { FilterColumnCondition, FilterCondition } from '../../utilities/filter/filter.util';

@Component({
  selector: 'app-table',
  standalone: true,
  imports: [
    CommonModule,
  ],
  templateUrl: './table.component.html',
  styleUrl: './table.component.scss'
})
export class TableComponent<TData extends object> {

  @Input({ required: true }) data!: TData[];

  @ContentChild('headers') headers!: TemplateRef<any>;

  @ContentChild('rows') rows!: TemplateRef<any>;

  private filters: { [name: string]: Observable<FilterColumnCondition> } = {};
  private filterSubscription?: Subscription;
  @Output() filterChange = new EventEmitter<FilterColumnCondition[]>();

  private readonly $sortTable: SortTableService;

  id = generateGuid();

  constructor(
    $sortTable: SortTableService,
  ) {
    this.$sortTable = $sortTable;
  }

  getDefaultHeaders(): string[] {
    if (this.data[0]) {
      const keys = Object.keys(this.data[0]);
      return keys;
    } else {
      return [];
    }
  }

  bindFilter(name: string, filter: Observable<FilterCondition>) {
    this.filters[name] = filter.pipe(
      startWith({ column: name, operator: undefined, value: undefined }),
      map(item => ({ column: name, ...item })),
    );

    this.regenerateFilters();
  }

  unbindFilter(name: string) {
    delete this.filters[name];

    this.regenerateFilters();
  }

  regenerateFilters() {
    this.filterSubscription?.unsubscribe();
    this.filterSubscription = undefined;

    const observables = Object.keys(this.filters)
      .map(key => this.filters[key].pipe(
        distinctUntilChanged((a, b) => isEqual(a, b)),
      ));

    this.filterSubscription = combineLatest(observables).pipe(
      debounceTime(0),
      map(filters => filters.filter(item => item.operator)),
    ).subscribe(value => {
      this.filterChange.emit(value);
    });
  }
}
