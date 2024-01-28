import { Directive, ElementRef, Input } from '@angular/core';
import { NavbarMenuAction } from '../../services/navbar-menu/navbar-menu.service';

@Directive({
  selector: '[searchAction]',
  standalone: true
})
export class NavbarActionSearchDirective {

  @Input({ required: true }) searchAction: NavbarMenuAction = undefined!;

  private $element: ElementRef;

  constructor($element: ElementRef) {
    this.$element = $element;
  }

  focus() {
    this.$element.nativeElement.value = null;
    this.$element.nativeElement.focus();
  }
}
