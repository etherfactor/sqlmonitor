import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
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
      text: {
        htmlContent: '<span>Content here</span><span>Content here</span>',
      },
    };
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render provided html', () => {
    const textDiv = fixture.debugElement.query(By.css('[data-testid="text-div"]'));
    expect(textDiv).toBeTruthy();

    expect(textDiv.childNodes.length).toBe(2);

    expect(textDiv.childNodes[0].nativeNode.nodeName).toBe('SPAN');
    expect(textDiv.childNodes[1].nativeNode.nodeName).toBe('SPAN');
  });
});
