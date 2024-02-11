import { ElementRef } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { NavbarActionSearchDirective } from './navbar-action-search.directive';

describe('NavbarActionSearchDirective', () => {
  let directive: NavbarActionSearchDirective;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NavbarActionSearchDirective]
    })
    .compileComponents();

    const $element = TestBed.inject(ElementRef);
    directive = new NavbarActionSearchDirective($element);
  });

  it('should create an instance', () => {
    expect(directive).toBeTruthy();
  });
});
