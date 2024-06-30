import { Component } from '@angular/core';
import { NgbActiveOffcanvas } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent {

  private readonly $activeOffcanvas: NgbActiveOffcanvas;

  constructor(
    $activeOffcanvas: NgbActiveOffcanvas,
  ) {
    this.$activeOffcanvas = $activeOffcanvas;
  }

  close() {
    this.$activeOffcanvas.close();
  }
}
