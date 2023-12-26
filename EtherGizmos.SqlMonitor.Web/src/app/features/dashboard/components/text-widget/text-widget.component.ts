import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ChartConfiguration } from 'chart.js';
import { DashboardWidget } from '../../models/dashboard-widget';

@Component({
  selector: 'text-widget',
  standalone: true,
  imports: [],
  templateUrl: './text-widget.component.html',
  styleUrl: './text-widget.component.scss'
})
export class TextWidgetComponent {

  private $modal: NgbModal;

  @Input({ required: true }) config: DashboardWidget = undefined!;
  @Output() configChange = new EventEmitter<DashboardWidget>();
  configChartType: ChartConfiguration['type'] = undefined!;
  configChartOptions: ChartConfiguration['options'] = undefined!;

  @Output() onDelete = new EventEmitter<DashboardWidget>();

  constructor(
    $modal: NgbModal,
  ) {
    this.$modal = $modal;
  }
}
