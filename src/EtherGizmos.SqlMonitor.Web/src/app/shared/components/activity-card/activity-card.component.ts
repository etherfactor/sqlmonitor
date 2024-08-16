import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'activity-card',
  standalone: true,
  imports: [
    CommonModule,
    NgbTooltipModule,
    RouterModule,
  ],
  templateUrl: './activity-card.component.html',
  styleUrl: './activity-card.component.scss'
})
export class ActivityCardComponent {

  @Input({ required: true }) title!: string;
  @Input() icon?: string;
  
  @Input() link?: string;

  @Input() isLoading: boolean = false;

  @Input() statuses: RecordStatus[] = [];

  get totalCount() {
    const count = this.statuses.reduce((count, status) => count + status.count, 0);
    return count;
  }
}

export interface RecordStatus {
  label: string;
  count: number;
  color: string;
  tooltip?: string;
}
