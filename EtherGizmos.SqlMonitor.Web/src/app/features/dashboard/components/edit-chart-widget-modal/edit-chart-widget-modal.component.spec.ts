import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { provideMetricServiceMock } from '../../../../shared/services/metric/metric.service.mock';
import { EditChartWidgetModalComponent } from './edit-chart-widget-modal.component';

describe('EditChartWidgetModalComponent', () => {
  let component: EditChartWidgetModalComponent;
  let fixture: ComponentFixture<EditChartWidgetModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        EditChartWidgetModalComponent,
      ],
      providers: [
        NgbActiveModal,
        provideMetricServiceMock(),
      ]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(EditChartWidgetModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
