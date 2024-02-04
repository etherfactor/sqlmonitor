import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, QueryList, ViewChildren } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { NgbModal, NgbModalModule } from '@ng-bootstrap/ng-bootstrap';
import 'chartjs-adapter-luxon';
import { GridStackOptions, GridStackWidget } from 'gridstack';
import { GridstackModule, nodesCB } from 'gridstack/dist/angular';
import { DateTime } from 'luxon';
import { BaseChartDirective, NgChartsModule } from 'ng2-charts';
import { QuillModule } from 'ngx-quill';
import { Subscription, asyncScheduler, interval, observeOn } from 'rxjs';
import { MetricDataService } from '../../../../shared/services/metric-data/metric-data.service';
import { NavbarMenuService } from '../../../../shared/services/navbar-menu/navbar-menu.service';
import { generateGuid } from '../../../../shared/types/guid/guid';
import { RelativeTimeInterpretation, evaluateRelativeTime, getTimeRangeText, interpretRelativeTime, parseRelativeTime } from '../../../../shared/types/relative-time/relative-time';
import { Bound } from '../../../../shared/utilities/bound/bound.util';
import { DefaultControlTypes, TypedFormGroup, getAllFormValues } from '../../../../shared/utilities/form/form.util';
import { Dashboard, dashboardForm } from '../../models/dashboard';
import { DashboardWidget, DashboardWidgetChartScaleType, DashboardWidgetChartType, DashboardWidgetType, dashboardWidgetForm } from '../../models/dashboard-widget';
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

  gridOptions: GridStackOptions = {
    margin: 5,
    cellHeight: 60,
  };

  updateTimeSubscription?: Subscription;

  dashboardForm?: TypedFormGroup<Dashboard, DefaultControlTypes>;

  startTimeInterpretation: RelativeTimeInterpretation = interpretRelativeTime(this.dashboardForm?.value?.timeStart ?? parseRelativeTime('t-1m'));
  startTime: DateTime = evaluateRelativeTime(this.startTimeInterpretation);

  endTimeInterpretation: RelativeTimeInterpretation = interpretRelativeTime(this.dashboardForm?.value?.timeEnd ?? parseRelativeTime('t'));
  endTime: DateTime = evaluateRelativeTime(this.endTimeInterpretation);

  gridItems: { [key: string]: GridStackWidget } = {};
  
  @ViewChildren(BaseChartDirective) charts: QueryList<BaseChartDirective> = undefined!;

  getAllFormValues = getAllFormValues;

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

    this.dashboardForm = dashboardForm(
      this.$form,
      {
        id: undefined!,
        timeStart: parseRelativeTime('t-1m'),
        timeEnd: parseRelativeTime('t'),
        items: [],
      }
    );

    this.updateTimeSubscription = interval(1000).pipe(
      observeOn(asyncScheduler),
    ).subscribe(() => {
      this.startTime = evaluateRelativeTime(this.startTimeInterpretation);
      this.endTime = evaluateRelativeTime(this.endTimeInterpretation);
    });

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

  private addWidget(item: DashboardWidget) {
    if (!this.dashboardForm)
      return;

    const form = dashboardWidgetForm(this.$form, item);
    this.dashboardForm.controls.items.push(form);
    this.dashboardForm.controls.items.markAsDirty();
  }

  @Bound selectInstance() {

  }

  @Bound selectTime() {
    const modal = this.$modal.open(SelectTimeModalComponent, { centered: true, backdrop: 'static', keyboard: false });
    const modalInstance = <SelectTimeModalComponent>modal.componentInstance;
    modalInstance.setTime({
      startTime: this.dashboardForm ? this.dashboardForm.value.timeStart! : parseRelativeTime('t-1m'),
      endTime: this.dashboardForm ? this.dashboardForm.value.timeEnd! : parseRelativeTime('t'),
    });

    modal.result.then(
      (result: TimeConfiguration) => {
        if (this.dashboardForm) {
          this.dashboardForm.controls.timeStart.setValue(result.startTime);
          this.startTimeInterpretation = interpretRelativeTime(this.dashboardForm.value.timeStart!);

          this.dashboardForm.controls.timeEnd.setValue(result.endTime);
          this.endTimeInterpretation = interpretRelativeTime(this.dashboardForm.value.timeEnd!);
        }

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
            label: getTimeRangeText(this.startTimeInterpretation, this.endTimeInterpretation),
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
    console.log('items', this.dashboardForm?.value?.items);
  }

  updateWidget(item: DashboardWidget) {
    if (!this.dashboardForm)
      return;

    const form = dashboardWidgetForm(this.$form, item);

    const index = this.dashboardForm.controls.items.controls.findIndex(e => e.value.id == item.id);
    if (index >= 0) {
      this.dashboardForm.controls.items.removeAt(index);
      this.dashboardForm.controls.items.insert(index, form);
      this.dashboardForm.controls.items.markAsDirty();
    }
  }

  deleteWidget(item: DashboardWidget) {
    if (!this.dashboardForm)
      return;

    let dirty: boolean = false;

    let index: number;
    while ((index = this.dashboardForm.controls.items.controls.findIndex(e => e.value.id == item.id)) !== -1) {
      this.dashboardForm.controls.items.removeAt(index);
      dirty = true;
    }

    if (dirty) {
      this.dashboardForm.controls.items.markAsDirty();
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

    if (!this.dashboardForm)
      return;

    const items: DashboardWidget[] = getAllFormValues(this.dashboardForm).items;

    for (const node of data.nodes) {
      const item = items.find(e => e.id === node.id);
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
