import { CommonModule } from '@angular/common';
import { Component, Input, forwardRef } from '@angular/core';
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from '@angular/forms';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { darken, lighten, readableColor, readableColorIsBlack, toRgba } from 'color2k';
import { ColorEvent } from 'ngx-color';
import { ColorSketchModule } from 'ngx-color/sketch';

@Component({
  selector: 'input-color-picker',
  standalone: true,
  imports: [
    CommonModule,
    ColorSketchModule,
    FormsModule,
    NgbDropdownModule,
  ],
  templateUrl: './input-color-picker.component.html',
  styleUrl: './input-color-picker.component.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => InputColorPickerComponent),
      multi: true
    }
  ]
})
export class InputColorPickerComponent implements ControlValueAccessor {

  @Input() id?: string;
  @Input() placeholder?: string;

  disabled: boolean = false;

  private _value: string = '#ffffff';
  get value() {
    return this._value;
  }

  set value(v: string) {
    if (v !== this.value && CSS.supports('color', v)) {
      this._value = v;
      this.onChanged(v);
    }
  }

  onChanged: (value: string) => void = () => { };
  onTouched: () => void = () => { };

  writeValue(obj: unknown): void {
    if (obj === null || obj === undefined) {
      this.value = '#ffffff';
    } else if (typeof obj === 'string') {
      this.value = obj;
    } else {
      throw new Error(`Unexpected type of input '${typeof obj}': ${obj}`);
    }
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChanged = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(disabled: boolean): void {
    this.disabled = disabled;
  }

  get dynamicPreviewStyle() {
    return {
      'background-color': this.value,
    };
  }

  get dynamicPickerStyle() {
    const base = this.value;
    const useLighten = !readableColorIsBlack(base);
    const text = readableColor(base);

    const hover = useLighten ? lighten(base, 0.05) : darken(base, 0.05);
    const active = useLighten ? lighten(base, 0.075) : darken(base, 0.075);
    const disabled = base;

    const shadowRgba = toRgba(base);
    const shadow = shadowRgba.substring(5, shadowRgba.length - 1)
      .replace(/ /g, '')
      .split(',')
      .slice(0, 3)
      .join(', ');

    return {
      '--bs-btn-color': text,
      '--bs-btn-bg': base,
      '--bs-btn-border-color': 'var(--bs-border-color)',
      '--bs-btn-hover-color': text,
      '--bs-btn-hover-bg': hover,
      '--bs-btn-hover-border-color': 'var(--bs-border-color)',
      '--bs-btn-focus-shadow-rgb': shadow,
      '--bs-btn-active-color': text,
      '--bs-btn-active-bg': active,
      '--bs-btn-active-border-color': 'var(--bs-border-color)',
      '--bs-btn-active-shadow': '',
      '--bs-btn-disabled-color': text,
      '--bs-btn-disabled-bg': disabled,
      '--bs-btn-disabled-border-color': 'var(--bs-border-color)',
    };
  }

  colorChanged($event: ColorEvent) {
    this._value = $event.color.hex;
  }
}
