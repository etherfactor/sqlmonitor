import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditTextWidgetModalComponent } from './edit-text-widget-modal.component';

describe('EditTextWidgetModalComponent', () => {
  let component: EditTextWidgetModalComponent;
  let fixture: ComponentFixture<EditTextWidgetModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditTextWidgetModalComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(EditTextWidgetModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
