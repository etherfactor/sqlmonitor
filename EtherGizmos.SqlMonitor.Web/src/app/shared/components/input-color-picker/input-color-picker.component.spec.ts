import { DebugElement } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
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

  it('should open dropdown when color picker is clicked', () => {
    let element: DebugElement;
    let styles: CSSStyleDeclaration;
    const button = fixture.debugElement.query(By.css('[data-testid="color-picker-button"]'));

    element = fixture.debugElement.query(By.css('[data-testid="color-picker"]'));
    styles = window.getComputedStyle(element.nativeElement);
    expect(styles['display']).toBe('none');

    button.nativeElement.click();
    fixture.detectChanges();

    element = fixture.debugElement.query(By.css('[data-testid="color-picker"]'));
    styles = window.getComputedStyle(element.nativeElement);
    expect(styles['display']).not.toBe('none');
  });

  it('should change color when value changes', () => {
    let buttonBackround: string;

    buttonBackround = component.dynamicPickerStyle['--bs-btn-bg'];
    expect(buttonBackround).toBe('#ffffff');

    component.writeValue('#ff0000');
    fixture.detectChanges();

    buttonBackround = component.dynamicPickerStyle['--bs-btn-bg'];
    expect(buttonBackround).toBe('#ff0000');
  });

  it('should bind to ngModel', () => {
    component.registerOnChange(value => {
      expect(value).toBe('#ff0000');
    });

    component.writeValue('#ff0000');
  });
});
