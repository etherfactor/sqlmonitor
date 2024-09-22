import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EditChartWidgetModalComponent } from '../../components/edit-chart-widget-modal/edit-chart-widget-modal.component';

@Component({
  selector: 'dashboard-detail',
  standalone: true,
  imports: [],
  templateUrl: './dashboard-detail.component.html',
  styleUrl: './dashboard-detail.component.scss'
})
export class DashboardDetailComponent implements OnInit {

  private readonly $modal: NgbModal;

  constructor(
    $modal: NgbModal,
  ) {
    this.$modal = $modal;
  }

  ngOnInit(): void {
    this.$modal.open(EditChartWidgetModalComponent, { size: 'lg', centered: true, backdrop: 'static', keyboard: false });
  }
}
