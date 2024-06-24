import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-delete-widget-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './delete-widget-modal.component.html',
  styleUrl: './delete-widget-modal.component.scss'
})
export class DeleteWidgetModalComponent {

  $activeModal: NgbActiveModal;

  constructor(
    $activeModal: NgbActiveModal
  ) {
    this.$activeModal = $activeModal;
  }
}
