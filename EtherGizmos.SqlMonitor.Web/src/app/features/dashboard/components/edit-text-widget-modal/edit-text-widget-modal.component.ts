import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { QuillModule } from 'ngx-quill';
import { DashboardWidget, dashboardWidgetForm } from '../../models/dashboard-widget';
import { TypedFormGroup } from '../../../../shared/utilities/form/form.util';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-edit-text-widget-modal',
  standalone: true,
  imports: [
    CommonModule,
    QuillModule,
    ReactiveFormsModule,
  ],
  templateUrl: './edit-text-widget-modal.component.html',
  styleUrl: './edit-text-widget-modal.component.scss'
})
export class EditTextWidgetModalComponent {

  private $form: FormBuilder;

  $activeModal: NgbActiveModal;

  widget?: DashboardWidget;
  widgetForm?: TypedFormGroup<DashboardWidget>;

  constructor(
    $activeModal: NgbActiveModal,
    $form: FormBuilder
  ) {
    this.$activeModal = $activeModal;
    this.$form = $form;
  }

  setWidget(item: DashboardWidget) {
    this.widget = item;
    this.widgetForm = dashboardWidgetForm(this.$form, item);
  }

  trySubmit() {
    if (!this.widgetForm)
      return;

    console.log(this.widgetForm.value);

    this.$activeModal.close(this.widgetForm.value);
  }
}
