import { ElementRef } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideElementRefMock } from '../../../testing/mocks/elementref.mock';
import { NavbarActionSearchDirective } from './navbar-action-search.directive';

describe('NavbarActionSearchDirective', () => {
  let directive: NavbarActionSearchDirective;
  let focused: boolean;
  let element: { value: string | null, focus: () => void };

  beforeEach(async () => {
    focused = false;
    element = {
      value: 'value',
      focus() {
        focused = true;
      }
    };

    await TestBed.configureTestingModule({
      imports: [NavbarActionSearchDirective],
      providers: [
        provideElementRefMock(element),
      ]
    })
    .compileComponents();

    const $element = TestBed.inject(ElementRef);
    directive = new NavbarActionSearchDirective($element);
  });

  it('should create an instance', () => {
    expect(directive).toBeTruthy();
  });

  it('should focus correctly', () => {
    expect(focused).toBeFalse();
    expect(element.value).toBe('value');

    directive.focus();

    expect(focused).toBeTrue();
    expect(element.value).toBe(null);
  });
});
