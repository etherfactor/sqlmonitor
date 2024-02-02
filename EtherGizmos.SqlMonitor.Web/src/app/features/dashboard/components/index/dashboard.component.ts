import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, QueryList, ViewChildren } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { NgbModal, NgbModalModule } from '@ng-bootstrap/ng-bootstrap';
import 'chartjs-adapter-luxon';
import { GridStackOptions, GridStackWidget } from 'gridstack';
import { GridstackModule, nodesCB } from 'gridstack/dist/angular';
import { BaseChartDirective, NgChartsModule } from 'ng2-charts';
import { QuillModule } from 'ngx-quill';
import { MetricDataService } from '../../../../shared/services/metric-data/metric-data.service';
import { NavbarMenuService } from '../../../../shared/services/navbar-menu/navbar-menu.service';
import { generateGuid } from '../../../../shared/types/guid/guid';
import { getTimeRangeText, interpretRelativeTime, parseRelativeTime } from '../../../../shared/types/relative-time/relative-time';
import { Bound } from '../../../../shared/utilities/bound/bound.util';
import { TypedFormGroup } from '../../../../shared/utilities/form/form.util';
import { DashboardWidget, DashboardWidgetChartScaleType, DashboardWidgetChartType, DashboardWidgetType } from '../../models/dashboard-widget';
import { ChartWidgetComponent } from '../chart-widget/chart-widget.component';
import { SelectTimeModalComponent, TimeConfiguration } from '../select-time-modal/select-time-modal.component';
import { TextWidgetComponent } from '../text-widget/text-widget.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    ChartWidgetComponent,
    CommonModule,
    GridstackModule,
    NgbModalModule,
    NgChartsModule,
    QuillModule,
    TextWidgetComponent,
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {

  private $cd: ChangeDetectorRef;
  private $form: FormBuilder;
  private $metricData: MetricDataService;
  private $modal: NgbModal;
  private $navbarMenu: NavbarMenuService;

  private widgetForm?: TypedFormGroup<DashboardWidget>;

  gridOptions: GridStackOptions = {
    margin: 5,
    cellHeight: 60,
  };

  startTime: string = 't-1m';
  endTime: string = 't';
  items: DashboardWidget[] = [];
  gridItems: { [key: string]: GridStackWidget } = {};
  
  @ViewChildren(BaseChartDirective) charts: QueryList<BaseChartDirective> = undefined!;

  constructor(
    $cd: ChangeDetectorRef,
    $form: FormBuilder,
    $metricData: MetricDataService,
    $modal: NgbModal,
    $navbarMenu: NavbarMenuService,
  ) {
    this.$cd = $cd;
    this.$form = $form;
    this.$metricData = $metricData;
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
        icon: 'bi-pc-display',
        label: 'All instances',
        callback: this.selectInstance,
      },
      {
        icon: 'bi-clock',
        label: 'Last 1 minute',
        callback: this.selectTime,
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

  getGridstackWidget(widget: DashboardWidget): GridStackWidget {
    let found = this.gridItems[widget.id];
    if (!found) {
      found = {
        id: widget.id,
        x: widget.grid?.xPos,
        y: widget.grid?.yPos,
        w: widget.grid?.width,
        h: widget.grid?.height,
      };

      this.gridItems[widget.id] = found;
    }

    return found;
  }

  identifyWidget(index: number, widget: DashboardWidget) {
    return widget.id;
  }

  @Bound addChart() {
    this.addWidget({
      id: generateGuid(),
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
        colors: ['#ffffff', '#ffffff', '#ffffff', '#ffffff', '#ffffff', '#ffffff', '#ffffff', '#ffffff'],
        xScale: {
          id: "X",
          type: DashboardWidgetChartScaleType.Time,
          minEnforced: false,
          maxEnforced: false,
          stacked: false,
        },
        yScales: [
          //{
          //  id: "Y1",
          //  type: DashboardWidgetChartScaleType.Linear,
          //  minEnforced: false,
          //  maxEnforced: false,
          //  stacked: false,
          //}
        ],
        metrics: [],
      },
    });
  }

  @Bound addText() {
    this.addWidget({
      id: generateGuid(),
      type: DashboardWidgetType.Text,
      grid: {
        xPos: 4,
        yPos: 0,
        width: 4,
        height: 5,
        hovering: false,
      },
      text: {
        htmlContent: "Text here",
      },
    });
  }

  private addWidget(widget: DashboardWidget) {
    this.items.push(widget);
  }

  @Bound selectInstance() {

  }

  @Bound selectTime() {
    const modal = this.$modal.open(SelectTimeModalComponent, { centered: true, backdrop: 'static', keyboard: false });
    const modalInstance = <SelectTimeModalComponent>modal.componentInstance;
    modalInstance.setTime({
      startTime: this.startTime,
      endTime: this.endTime,
    });

    modal.result.then(
      (result: TimeConfiguration) => {
        this.startTime = result.startTime;
        this.endTime = result.endTime;

        const interpretedStart = interpretRelativeTime(parseRelativeTime(this.startTime));
        const interpretedEnd = interpretRelativeTime(parseRelativeTime(this.endTime));

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
            icon: 'bi-pc-display',
            label: 'All instances',
            callback: this.selectInstance,
          },
          {
            icon: 'bi-clock',
            label: getTimeRangeText(interpretedStart, interpretedEnd),
            callback: this.selectTime,
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
      },
      cancel => { }
    );
  }

  @Bound printWidgets() {
    console.log(this.items);
  }

  updateWidget(item: DashboardWidget) {
    const index = this.items.findIndex(e => e.id == item.id);
    if (index >= 0) {
      this.items[index] = item;
    }
  }

  deleteWidget(item: DashboardWidget) {
    let index: number;
    while ((index = this.items.indexOf(item)) !== -1) {
      this.items.splice(index, 1);
    }
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

    for (const node of data.nodes) {
      const item = this.items.find(e => e.id === node.id);
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

      const gridItem = this.getGridstackWidget(item);
      gridItem.x = node.x;
      gridItem.y = node.y;
      gridItem.w = node.w;
      gridItem.h = node.h;
    }
  }
}
