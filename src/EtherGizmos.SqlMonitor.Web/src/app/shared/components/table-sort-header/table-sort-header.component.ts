import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Direction } from '../../utilities/odata/odata.util';

@Component({
  selector: '[app-table-sort-header]',
  standalone: true,
  imports: [
    CommonModule,
  ],
  templateUrl: './table-sort-header.component.html',
  styleUrl: './table-sort-header.component.scss',
  host: {
    '(click)': 'onClick($event)',
    '(mouseenter)': 'onMouseEnter($event)',
    '(mouseleave)': 'onMouseLeave($event)',
  }
})
export class TableSortHeaderComponent {

  direction?: Direction;

  onClick() {
    if (this.direction) {
      if (this.direction === 'asc') {
        this.direction = 'desc';
      } else {
        this.direction = 'asc';
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
