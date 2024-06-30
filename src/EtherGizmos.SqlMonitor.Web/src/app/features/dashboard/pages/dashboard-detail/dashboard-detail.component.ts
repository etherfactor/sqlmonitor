import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, QueryList, ViewChildren } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { NgbModal, NgbModalModule } from '@ng-bootstrap/ng-bootstrap';
import 'chartjs-adapter-luxon';
import { GridStackOptions, GridStackWidget } from 'gridstack';
import { GridstackModule, nodesCB } from 'gridstack/dist/angular';
import { DateTime } from 'luxon';
import { BaseChartDirective } from 'ng2-charts';
import { QuillModule } from 'ngx-quill';
import { Observable, Subscription, asyncScheduler, first, interval, map, observeOn, of, shareReplay } from 'rxjs';
import { BodyContainerType, BodyService } from '../../../../shared/services/body/body.service';
import { DashboardService } from '../../../../shared/services/dashboard/dashboard.service';
import { InstanceService } from '../../../../shared/services/instance/instance.service';
import { MetricDataService } from '../../../../shared/services/metric-data/metric-data.service';
import { NavbarMenuService } from '../../../../shared/services/navbar-menu/navbar-menu.service';
import { Guid, generateGuid, isGuid } from '../../../../shared/types/guid/guid';
import { RelativeTimeInterpretation, evaluateRelativeTime, getTimeRangeText, interpretRelativeTime, parseRelativeTime } from '../../../../shared/types/relative-time/relative-time';
import { Bound } from '../../../../shared/utilities/bound/bound.util';
import { DefaultControlTypes, TypedFormGroup, getAllFormValues } from '../../../../shared/utilities/form/form.util';
import { ChartWidgetComponent } from '../../components/chart-widget/chart-widget.component';
import { SelectInstanceModalComponent } from '../../components/select-instance-modal/select-instance-modal.component';
import { SelectTimeModalComponent, TimeConfiguration } from '../../components/select-time-modal/select-time-modal.component';
import { TextWidgetComponent } from '../../components/text-widget/text-widget.component';
import { Dashboard, dashboardForm } from '../../models/dashboard';
import { DashboardWidget, DashboardWidgetChartScaleType, DashboardWidgetChartType, DashboardWidgetType, dashboardWidgetForm } from '../../models/dashboard-widget';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    BaseChartDirective,
    ChartWidgetComponent,
    CommonModule,
    GridstackModule,
    NgbModalModule,
    QuillModule,
    TextWidgetComponent,
  ],
  templateUrl: './dashboard-detail.component.html',
  styleUrl: './dashboard-detail.component.scss'
})
export class DashboardDetailComponent implements OnInit {

  private readonly $activatedRoute: ActivatedRoute;
  private readonly $body: BodyService;
  private readonly $cd: ChangeDetectorRef;
  private readonly $dashboard: DashboardService;
  private readonly $form: FormBuilder;
  private readonly $instance: InstanceService;
  private readonly $metricData: MetricDataService;
  private readonly $modal: NgbModal;
  private readonly $navbarMenu: NavbarMenuService;

  isEditing: boolean = false;

  dashboard$?: Observable<Dashboard>;
  dashboard?: Dashboard;
  dashboardForm?: TypedFormGroup<Dashboard, DefaultControlTypes>;

  gridOptions: GridStackOptions = {
    margin: 5,
    cellHeight: 60,
  };

  updateTimeSubscription?: Subscription;

  startTimeInterpretation: RelativeTimeInterpretation = interpretRelativeTime(this.dashboardForm?.value?.timeStart ?? parseRelativeTime('t-1m'));
  startTime: DateTime = evaluateRelativeTime(this.startTimeInterpretation);

  endTimeInterpretation: RelativeTimeInterpretation = interpretRelativeTime(this.dashboardForm?.value?.timeEnd ?? parseRelativeTime('t'));
  endTime: DateTime = evaluateRelativeTime(this.endTimeInterpretation);

  instanceFilters: Guid[] = [];

  gridItems: { [key: string]: GridStackWidget } = {};
  
  @ViewChildren(BaseChartDirective) charts: QueryList<BaseChartDirective> = undefined!;

  getAllFormValues = getAllFormValues;

  constructor(
    $activatedRoute: ActivatedRoute,
    $body: BodyService,
    $cd: ChangeDetectorRef,
    $dashboard: DashboardService,
    $form: FormBuilder,
    $instance: InstanceService,
    $metricData: MetricDataService,
    $modal: NgbModal,
    $navbarMenu: NavbarMenuService,
  ) {
    this.$activatedRoute = $activatedRoute;
    this.$body = $body;
    this.$cd = $cd;
    this.$dashboard = $dashboard;
    this.$form = $form;
    this.$instance = $instance;
    this.$metricData = $metricData;
    this.$modal = $modal;
    this.$navbarMenu = $navbarMenu;
  }

  ngOnInit(): void {

    this.dashboardForm = dashboardForm(
      this.$form,
      {
        id: undefined!,
        name: 'Unnamed',
        timeStart: parseRelativeTime('t-1m'),
        timeEnd: parseRelativeTime('t'),
        instanceIds: [],
        items: [],
      }
    );

    this.updateTimeSubscription = interval(1000).pipe(
      observeOn(asyncScheduler),
    ).subscribe(() => {
      this.startTime = evaluateRelativeTime(this.startTimeInterpretation);
      this.endTime = evaluateRelativeTime(this.endTimeInterpretation);
    });

    this.$body.setContainer(BodyContainerType.Fluid);
    this.updateBreadcrumbs();
    this.updateActions();

    let observable: Observable<Dashboard>;
    if (this.$activatedRoute.snapshot.params['id']) {
      const id = this.$activatedRoute.snapshot.params['id'];
      if (isGuid(id)) {
        observable = this.$dashboard.get(id);
      } else {
        throw new Error('Invalid id format');
      }
    } else {
      observable = of({
        id: generateGuid(),
        name: 'Unnamed',
        timeStart: parseRelativeTime('t-5m'),
        timeEnd: parseRelativeTime('t'),
        instanceIds: [],
        items: [],
      });
    }

    this.dashboard$ = observable.pipe(
      shareReplay({ bufferSize: 1, refCount: false })
    );

    this.dashboard$.pipe(
      first()
    ).subscribe(dashboard => {
      this.dashboard = dashboard;
      this.initForm();
    });
  }

  private initForm() {
    this.dashboardForm = dashboardForm(this.$form, this.dashboard);
    this.instanceFilters = this.dashboardForm?.value?.instanceIds ?? [];
    this.startTimeInterpretation = interpretRelativeTime(this.dashboardForm?.value?.timeStart ?? parseRelativeTime('t-1m'));
    this.endTimeInterpretation = interpretRelativeTime(this.dashboardForm?.value?.timeEnd ?? parseRelativeTime('t'));

    this.updateBreadcrumbs();
    this.updateActions();
  }

  private updateBreadcrumbs() {
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
        label: this.getDashboardBreadcrumbName(),
        link: this.getDashboardBreadcrumbPath(),
      },
    ]);
  }

  private updateActions() {
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
        label: this.getInstanceActionName(),
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
  }

  getDashboardBreadcrumbName() {
    if (this.dashboard$) {
      return this.dashboard$.pipe(
        map(item => item.name),
      );
    } else {
      return 'Unknown';
    }
  }

  getDashboardBreadcrumbPath() {
    if (this.dashboard$) {
      return this.dashboard$.pipe(
        map(item => `/dashboard/${item.id}`),
      );
    } else {
      return '/dashboards';
    }
  }

  getInstanceActionName(): string | Observable<string> {
    if (this.dashboardForm) {
      const count = this.dashboardForm.controls.instanceIds.controls.length;
      if (count > 1) {
        return `${count} instances`;
      } else if (count == 0) {
        return 'All instances';
      } else {
        return this.$instance.get(this.dashboardForm.controls.instanceIds.value[0]).pipe(
          map(instance => instance.name),
          shareReplay({ bufferSize: 1, refCount: false }),
        );
      }
    } else {
      return 'All instances';
    }
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
    const modal = this.$modal.open(SelectInstanceModalComponent, { centered: true, backdrop: 'static', keyboard: false });
    const modalInstance = <SelectInstanceModalComponent>modal.componentInstance;
    modalInstance.setInstanceIds(this.dashboardForm ? this.dashboardForm.value.instanceIds! : this.instanceFilters);

    modal.result.then(
      (results: Guid[]) => {
        if (this.dashboardForm) {
          this.dashboardForm.controls.instanceIds.clear();
          for (const result of results) {
            this.dashboardForm.controls.instanceIds.push(this.$form.nonNullable.control(result));
          }

          this.instanceFilters = results;
        }

        this.updateActions();
      },
      cancel => { }
    );
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

        this.updateActions();
      },
      cancel => { }
    );
  }

  @Bound printWidgets() {
    console.log('dashboard', this.dashboardForm?.value);
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
