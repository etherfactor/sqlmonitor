import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ChartConfiguration } from 'chart.js';
import { DashboardWidget } from '../../models/dashboard-widget';
import { DeleteWidgetModalComponent } from '../delete-widget-modal/delete-widget-modal.component';
import { EditTextWidgetModalComponent } from '../edit-text-widget-modal/edit-text-widget-modal.component';

@Component({
  selector: 'text-widget',
  standalone: true,
  imports: [],
  templateUrl: './text-widget.component.html',
  styleUrl: './text-widget.component.scss'
})
export class TextWidgetComponent implements OnInit {

  private $modal: NgbModal;

  @Input({ required: true }) config: DashboardWidget = undefined!;
  configChartType: ChartConfiguration['type'] = undefined!;
  configChartOptions: ChartConfiguration['options'] = undefined!;

  @Output() onUpdate = new EventEmitter<DashboardWidget>();
  @Output() onDelete = new EventEmitter<DashboardWidget>();

  constructor(
    $modal: NgbModal,
  ) {
    this.$modal = $modal;
  }

  ngOnInit(): void {
    this.updateWidget(this.config);
  }

  editWidget() {
    const modal = this.$modal.open(EditTextWidgetModalComponent, { centered: true, size: 'lg', backdrop: 'static', keyboard: false });
    const modalInstance = <EditTextWidgetModalComponent>modal.componentInstance;
    modalInstance.setWidget(this.config);

    modal.result.then(
      (result: DashboardWidget) => {
        this.updateWidget(result);
        this.onUpdate.next(result);
      },
      cancel => { }
    );
  }

  private updateWidget(newConfig: DashboardWidget) {
    this.config = newConfig;
  }

  deleteWidget() {
    this.$modal.open(DeleteWidgetModalComponent, { centered: true }).result.then(
      close => this.onDelete.next(this.config),
      cancel => { }
    );
  }
}
