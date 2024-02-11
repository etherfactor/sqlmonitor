import { ComponentFixture, TestBed } from '@angular/core/testing';
import { generateGuid } from '../../../../shared/types/guid/guid';
import { DashboardWidgetType } from '../../models/dashboard-widget';
import { TextWidgetComponent } from './text-widget.component';

describe('TextWidgetComponent', () => {
  let component: TextWidgetComponent;
  let fixture: ComponentFixture<TextWidgetComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TextWidgetComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(TextWidgetComponent);
    component = fixture.componentInstance;
    component.config = {
      id: generateGuid(),
      type: DashboardWidgetType.Text,
      grid: {
        xPos: 0,
        yPos: 0,
        width: 1,
        height: 1,
        hovering: false,
      },
    };
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
