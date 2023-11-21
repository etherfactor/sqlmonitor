import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { NgbModal, NgbModalModule } from '@ng-bootstrap/ng-bootstrap';
import { ChartConfiguration, ChartType } from 'chart.js';
import { GridStackOptions } from 'gridstack';
import { GridstackModule, nodesCB } from 'gridstack/dist/angular';
import { BaseChartDirective, NgChartsModule } from 'ng2-charts';
import { v4 as uuidv4 } from 'uuid';
import { NavbarMenuService } from '../../../../shared/services/navbar-menu/navbar-menu.service';
import { Bound } from '../../../../shared/utilities/bound/bound.util';
import { DashboardWidget } from '../../models/dashboard-widget';
import { DeleteWidgetModalComponent } from '../delete-widget-modal/delete-widget-modal.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    GridstackModule,
    NgbModalModule,
    NgChartsModule,
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {

  private $modal: NgbModal;
  private $navbarMenu: NavbarMenuService;

  gridOptions: GridStackOptions = {
    margin: 5,
    cellHeight: 120,
  };

  items: DashboardWidget[] = [];

  constructor(
    $modal: NgbModal,
    $navbarMenu: NavbarMenuService,
  ) {
    this.$modal = $modal;
    this.$navbarMenu = $navbarMenu;
  }

  ngOnInit(): void {
    this.$navbarMenu.setActions([
      {
        icon: 'bi-save',
        label: 'Save',
      },
      {
        icon: 'bi-x-square',
        label: 'Cancel',
      },
      {
        icon: 'bi-three-dots',
        label: 'More',
        subActions: [
          {
            icon: 'bi-bar-chart',
            label: 'Add Chart',
            callback: this.addWidget,
          },
          {
            icon: 'bi-type',
            label: 'Add Text (not impl.)',
          },
          {
            divider: true,
          },
          {
            icon: 'bi-printer',
            label: 'Debug Dashboard',
            callback: this.printWidgets,
          },
          {
            divider: true,
          },
          {
            icon: 'bi-trash',
            label: 'Delete Dashboard (not impl.)',
          },
        ],
      },
    ]);
  }

  identify(index: number, widget: DashboardWidget) {
    return widget.id;
  }

  @Bound addWidget() {
    this.items.push({
      id: uuidv4(),
      w: 2,
      h: 2,
    });
  }

  @Bound printWidgets() {
    console.log(this.items);
  }

  deleteWidget(item: DashboardWidget) {
    let index: number;
    while ((index = this.items.indexOf(item)) !== -1) {
      this.items.splice(index, 1);
    }
  }

  deleteWidgetModal(item: DashboardWidget) {
    this.$modal.open(DeleteWidgetModalComponent, { centered: true }).result.then(
      close => this.deleteWidget(item),
      cancel => { }
    );
  }

  onChange(data: nodesCB) {
    for (let node of data.nodes) {
      let item = this.items.find(e => e.id === node.id);
      if (!item)
        continue;

      item.x = node.x;
      item.y = node.y;
      item.w = node.w;
      item.h = node.h;
    }
  }

  public lineChartData: ChartConfiguration['data'] = {
    datasets: [
      {
        data: [65, 59, 80, 81, 56, 55, 40],
        label: 'Series A',
        backgroundColor: 'rgba(148,159,177,0.2)',
        borderColor: 'rgba(148,159,177,1)',
        pointBackgroundColor: 'rgba(148,159,177,1)',
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: 'rgba(148,159,177,0.8)',
        fill: 'origin',
      },
      {
        data: [28, 48, 40, 19, 86, 27, 90],
        label: 'Series B',
        backgroundColor: 'rgba(77,83,96,0.2)',
        borderColor: 'rgba(77,83,96,1)',
        pointBackgroundColor: 'rgba(77,83,96,1)',
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: 'rgba(77,83,96,1)',
        fill: 'origin',
      },
      {
        data: [180, 480, 770, 90, 1000, 270, 400],
        label: 'Series C',
        yAxisID: 'y1',
        backgroundColor: 'rgba(255,0,0,0.3)',
        borderColor: 'red',
        pointBackgroundColor: 'rgba(148,159,177,1)',
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: 'rgba(148,159,177,0.8)',
        fill: 'origin',
      },
    ],
    labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
  };

  public lineChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    elements: {
      line: {
        tension: 0.5,
      },
    },
    scales: {
      // We use this empty structure as a placeholder for dynamic theming.
      y: {
        position: 'left',
      },
      y1: {
        position: 'right',
        grid: {
          color: 'rgba(255,0,0,0.3)',
        },
        ticks: {
          color: 'red',
        },
      },
    },

    plugins: {
      legend: { display: true },
      //annotation: {
      //  annotations: [
      //    {
      //      type: 'line',
      //      scaleID: 'x',
      //      value: 'March',
      //      borderColor: 'orange',
      //      borderWidth: 2,
      //      label: {
      //        display: true,
      //        position: 'center',
      //        color: 'orange',
      //        content: 'LineAnno',
      //        font: {
      //          weight: 'bold',
      //        },
      //      },
      //    },
      //  ],
      //},
    },
  };

  public lineChartType: ChartType = 'line';

  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;
}
