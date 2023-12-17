import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InputColorPickerComponent } from './input-color-picker.component';

describe('InputColorPickerComponent', () => {
  let component: InputColorPickerComponent;
  let fixture: ComponentFixture<InputColorPickerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InputColorPickerComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(InputColorPickerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
