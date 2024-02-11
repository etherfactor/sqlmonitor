import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideInstanceServiceMock } from '../../../../shared/services/instance/instance.service.mock';
import { provideMetricDataServiceMock } from '../../../../shared/services/metric-data/metric-data.service.mock';
import { provideMetricServiceMock } from '../../../../shared/services/metric/metric.service.mock';
import { generateGuid } from '../../../../shared/types/guid/guid';
import { DashboardWidgetChartScaleType, DashboardWidgetType } from '../../models/dashboard-widget';
import { ChartWidgetComponent } from './chart-widget.component';

describe('ChartWidgetComponent', () => {
  let component: ChartWidgetComponent;
  let fixture: ComponentFixture<ChartWidgetComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        ChartWidgetComponent,
      ],
      providers: [
        provideInstanceServiceMock(),
        provideMetricServiceMock(),
        provideMetricDataServiceMock(),
      ]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ChartWidgetComponent);
    component = fixture.componentInstance;
    component.config = {
      id: generateGuid(),
      type: DashboardWidgetType.Chart,
      grid: {
        xPos: 0,
        yPos: 0,
        width: 1,
        height: 1,
        hovering: false,
      },
      chart: {
        colors: [],
        xScale: {
          id: 'X',
          type: DashboardWidgetChartScaleType.Time,
          minEnforced: false,
          maxEnforced: false,
          stacked: false,
        },
        yScales: [],
        metrics: [],
      },
    };
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
