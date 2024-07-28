import { CommonModule } from '@angular/common';
import { Component, Input, OnDestroy, OnInit, Optional } from '@angular/core';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { NgxMaskDirective } from 'ngx-mask';
import { SortTableService } from '../../services/sort-table/sort-table.service';
import { generateGuid } from '../../types/guid/guid';
import { Direction } from '../../utilities/odata/odata.util';
import { FilterType } from '../filter-builder-modal/filter-builder-modal.component';
import { InputLuxonDatetimeComponent } from '../input-luxon-datetime/input-luxon-datetime.component';
import { TableComponent } from '../table/table.component';

@Component({
  selector: '[app-table-header]',
  standalone: true,
  imports: [
    CommonModule,
    InputLuxonDatetimeComponent,
    NgbDropdownModule,
    NgxMaskDirective,
  ],
  templateUrl: './table-header.component.html',
  styleUrl: './table-header.component.scss',
  host: {
    //'(click)': 'onClick($event)',
    //'(mouseenter)': 'onMouseEnter($event)',
    //'(mouseleave)': 'onMouseLeave($event)',
  }
})
export class TableHeaderComponent<TData extends object> implements OnInit, OnDestroy {

  @Input({ alias: 'app-table-header', required: true }) name!: string;

  @Input() type?: FilterType;

  @Input() sortable: boolean = false;

  @Input() filterable: boolean = false;

  private readonly $sortTable: SortTableService;
  private readonly table: TableComponent<TData>;

  private id = generateGuid();

  get direction() {
    return this.$sortTable.getSortDirection(this.id, this.name);
  }

  set direction(value: Direction | undefined) {
    if (value) {
      this.$sortTable.setSortDirection(this.id, { column: this.name, direction: value });
    } else {
      this.$sortTable.setSortDirection(this.id, undefined);
    }
  }

  guidPatterns = {
    'X': { pattern: /[0-9A-Fa-f]/ },
    '4': { pattern: /4/ },
    '8': { pattern: /[8-9A-Ba-b]/ }
  };

  constructor(
    $sortTable: SortTableService,
    @Optional() table: TableComponent<TData>,
  ) {
    this.$sortTable = $sortTable;
    this.table = table;
  }

  ngOnInit(): void {
    if (this.table) {
      this.id = this.table.id;
    }
  }

  ngOnDestroy(): void {
    this.$sortTable.setSortDirection(this.id, undefined);
  }

  stopPropagation($event: Event) {
    $event.stopPropagation();
  }

  onClick() {
    if (this.direction) {
      if (this.direction === 'asc') {
        this.direction = 'desc';
      } else {
        this.direction = undefined;
      }
    } else {
      this.direction = 'asc';
    }
  }

  onMouseEnter() {
    console.log('enter');
  }

  onMouseLeave() {
    console.log('leave');
  }
}
