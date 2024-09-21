import { STEPPER_GLOBAL_OPTIONS } from '@angular/cdk/stepper';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatStepperModule } from '@angular/material/stepper';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'edit-chart-widget-modal',
  standalone: true,
  imports: [
    MatStepperModule,
    ReactiveFormsModule,
  ],
  providers: [
    {
      provide: STEPPER_GLOBAL_OPTIONS,
      useValue: { showError: true },
    },
  ],
  templateUrl: './edit-chart-widget-modal.component.html',
  styleUrl: './edit-chart-widget-modal.component.scss'
})
export class EditChartWidgetModalComponent {

  private readonly $activeModal: NgbActiveModal;
  private readonly $form: FormBuilder;

  firstFormGroup: FormGroup;
  secondFormGroup: FormGroup;

  constructor(
    $activeModal: NgbActiveModal,
    $form: FormBuilder,
  ) {
    this.$activeModal = $activeModal;
    this.$form = $form;

    this.firstFormGroup = this.$form.group({ firstCtrl: ['', Validators.required] });
    this.secondFormGroup = this.$form.group({ secondCtrl: [''] });
  }

  cancel() {
    this.$activeModal.dismiss();
  }

  trySubmit() {
    this.$activeModal.close();
  }
}
