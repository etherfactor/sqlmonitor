import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { NgbModal, NgbModalModule } from '@ng-bootstrap/ng-bootstrap';
import { ChartConfiguration, ChartType } from 'chart.js';
import { GridStackOptions } from 'gridstack';
import { GridstackModule, nodesCB } from 'gridstack/dist/angular';
import { BaseChartDirective, NgChartsModule } from 'ng2-charts';
import { v4 as uuidv4 } from 'uuid';
import { NavbarMenuService } from '../../../../shared/services/navbar-menu/navbar-menu.service';
import { Bound } from '../../../../shared/utilities/bound/bound.util';
import { TypedFormGroup } from '../../../../shared/utilities/form/form.util';
import { DashboardWidget, DashboardWidgetChartScaleType, DashboardWidgetChartType, DashboardWidgetType, GridstackDashboardWidget } from '../../models/dashboard-widget';
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

  private $form: FormBuilder;
  private $modal: NgbModal;
  private $navbarMenu: NavbarMenuService;

  private widgetForm?: TypedFormGroup<DashboardWidget>;

  gridOptions: GridStackOptions = {
    margin: 5,
    cellHeight: 60,
  };

  items: DashboardWidget[] = [];
  gridItems: GridstackDashboardWidget[] = [];

  constructor(
    $form: FormBuilder,
    $modal: NgbModal,
    $navbarMenu: NavbarMenuService,
  ) {
    this.$form = $form;
    this.$modal = $modal;
    this.$navbarMenu = $navbarMenu;
  }

  ngOnInit(): void {
    this.$navbarMenu.setBreadcrumbs([
      {
        label: 'Home',
        link: '/',
      },
      {
        label: 'Dashboards',
        link: '/dashboards',
      },
      {
        label: 'Test',
        link: '/dashboards/123',
      },
    ]);

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
        icon: 'bi-plus-square',
        label: 'Add',
        subActions: [
          {
            icon: 'bi-bar-chart',
            label: 'Chart',
            callback: this.addChart,
          },
          {
            icon: 'bi-type',
            label: 'Text',
            callback: this.addText,
          },
        ]
      },
      {
        icon: 'bi-three-dots',
        label: 'More',
        subActions: [
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

  getGridstackWidget(widget: DashboardWidget): GridstackDashboardWidget {
    return <GridstackDashboardWidget> {
      id: widget.id,
      x: widget.grid?.xPos,
      y: widget.grid?.yPos,
      w: widget.grid?.width,
      h: widget.grid?.height,
      type: widget.type,
      chart: widget.chart,
      text: widget.text,
    };
  }

  findGridstackWidget(widget: DashboardWidget): GridstackDashboardWidget {
    return this.gridItems.find(e => e.id == widget.id)!;
  }

  identify(index: number, widget: DashboardWidget) {
    return widget.id;
  }

  @Bound addChart() {
    this.addWidget({
      id: uuidv4(),
      type: DashboardWidgetType.Chart,
      grid: {
        xPos: 4,
        yPos: 0,
        width: 4,
        height: 5,
        hovering: false,
      },
      chart: {
        type: DashboardWidgetChartType.Line,
        xScale: {
          id: "x",
          type: DashboardWidgetChartScaleType.Time,
          minEnforced: false,
          maxEnforced: false,
        },
        yScales: [
          {
            id: "y",
            type: DashboardWidgetChartScaleType.Linear,
            minEnforced: false,
            maxEnforced: false,
          }
        ],
      },
    });
  }

  @Bound addText() {
    this.addWidget({
      id: uuidv4(),
      type: DashboardWidgetType.Text,
      grid: {
        xPos: 4,
        yPos: 0,
        width: 4,
        height: 5,
        hovering: false,
      },
      text: {
        htmlContent: "<i>Text here</i>",
      },
    });
  }

  private addWidget(widget: DashboardWidget) {
    this.items.push(widget);
    const gridWidget = this.getGridstackWidget(widget);
    console.log('from', widget, 'to', gridWidget);
    this.gridItems.push(gridWidget);
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

  onAdd(data: nodesCB) {
    this.populateNodes(data);
  }

  onChange(data: nodesCB) {
    this.populateNodes(data);
  }

  onRemove(data: nodesCB) {
    this.populateNodes(data);
  }

  private populateNodes(data: nodesCB) {
    console.log(data);

    for (let node of data.nodes) {
      let item = this.items.find(e => e.id === node.id);
      if (!item)
        continue;

      if (!item.grid) {
        item.grid = {
          xPos: 0,
          yPos: 0,
          width: 1,
          height: 1,
          hovering: false,
        };
      }

      item.grid.xPos = node.x || 0;
      item.grid.yPos = node.y || 0;
      item.grid.width = node.w || 0;
      item.grid.height = node.h || 0;
    }

    this.gridItems = this.items.map(item => this.getGridstackWidget(item));
  }

  public lineChartData: ChartConfiguration['data'] = {
    datasets: [
      {
        data: [
          { x: 0, y: 65, },
          { x: 1, y: 59, },
          { x: 2, y: 80, },
          { x: 3, y: 81, },
          { x: 4, y: 56, },
          { x: 5, y: 55, },
          { x: 6, y: 40, },
        ],
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
        data: [
          { x: 0, y: 28, },
          { x: 1, y: 48, },
          { x: 2, y: 40, },
          { x: 3, y: 19, },
          { x: 4, y: 86, },
          { x: 5, y: 27, },
          { x: 6, y: 90, },
        ],
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
        data: [
          { x: 0, y: 180, },
          { x: 1, y: 480, },
          { x: 2, y: 770, },
          { x: 3, y: 90, },
          { x: 4, y: 1000, },
          { x: 5, y: 270, },
          { x: 6, y: 400, },
        ],
        label: 'Series C',
        yAxisID: 'y1',
        backgroundColor: 'rgba(255,0,0,0.3)',
        borderColor: 'red',
        pointBackgroundColor: 'rgba(255,159,177,1)',
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: 'rgba(255,159,177,0.8)',
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
