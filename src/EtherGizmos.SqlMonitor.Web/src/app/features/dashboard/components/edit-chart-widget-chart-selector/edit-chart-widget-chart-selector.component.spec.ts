import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditChartWidgetChartSelectorComponent } from './edit-chart-widget-chart-selector.component';

describe('EditChartWidgetChartSelectorComponent', () => {
  let component: EditChartWidgetChartSelectorComponent;
  let fixture: ComponentFixture<EditChartWidgetChartSelectorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditChartWidgetChartSelectorComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditChartWidgetChartSelectorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
