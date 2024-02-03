import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { RelativeTime, interpretRelativeTime, isRelativeTime, isRelativeTimeValidator } from '../../../../shared/types/relative-time/relative-time';

@Component({
  selector: 'app-select-time-modal',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
  ],
  templateUrl: './select-time-modal.component.html',
  styleUrl: './select-time-modal.component.scss'
})
export class SelectTimeModalComponent {

  $activeModal: NgbActiveModal;

  private $form: FormBuilder;

  regex = /^\s*?t(?:\s*?(?<ASIGN>[+-])\s*?(?<AVALUE>\d+)\s*?(?<AUNIT>Y|M|w|d|h|m|s))?(?:\s*?@\s*?(?<RTYPE>[SsEe])[Oo](?<RUNIT>Y|M|w|d|h|m|s))?\s*?$/;
  timeAbbreviations: { [key: string]: string } = {
    Y: 'year',
    M: 'month',
    w: 'week',
    d: 'day',
    h: 'hour',
    m: 'minute',
    s: 'second',
  };

  timeStartForm?: FormControl<string>;
  timeEndForm?: FormControl<string>;

  constructor(
    $activeModal: NgbActiveModal,
    $form: FormBuilder,
  ) {
    this.$activeModal = $activeModal;
    this.$form = $form;
  }

  setTime(config: TimeConfiguration) {
    this.timeStartForm = this.$form.nonNullable.control(config.startTime, [Validators.required, isRelativeTimeValidator]);
    this.timeEndForm = this.$form.nonNullable.control(config.endTime, [Validators.required, isRelativeTimeValidator]);
  }

  getTimeLabel(value: string) {
    if (!isRelativeTime(value))
      return 'Invalid syntax';

    return interpretRelativeTime(value).friendlyText;
  }

  trySubmit() {
    if (!this.timeStartForm)
      return;

    if (!this.timeEndForm)
      return;

    const timeStartValue = this.timeStartForm.value;
    if (!isRelativeTime(timeStartValue))
      return;

    const timeEndValue = this.timeEndForm.value;
    if (!isRelativeTime(timeEndValue))
      return;

    const result: TimeConfiguration = {
      startTime: timeStartValue,
      endTime: timeEndValue,
    };

    this.$activeModal.close(result);
  }
}

export interface TimeConfiguration {
  startTime: RelativeTime;
  endTime: RelativeTime;
}
