import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditChartWidgetModalComponent } from './edit-chart-widget-modal.component';

describe('EditChartWidgetModalComponent', () => {
  let component: EditChartWidgetModalComponent;
  let fixture: ComponentFixture<EditChartWidgetModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditChartWidgetModalComponent]
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
