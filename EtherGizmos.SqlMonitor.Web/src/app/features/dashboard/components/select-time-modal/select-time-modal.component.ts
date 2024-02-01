import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

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

  regex = /^\s*?t(?:\s*?(?<ASIGN>[+-])\s*?(?<AVALUE>\d+)\s*?(?<AUNIT>Y|M|w|d|h|m|s)(?:\s*?@\s*?(?<RTYPE>[SsEe])[Oo](?<RUNIT>Y|M|w|d|h|m|s))?)?\s*?$/;
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
    this.timeStartForm = this.$form.nonNullable.control(config.startTime);
    this.timeEndForm = this.$form.nonNullable.control(config.endTime);
  }

  interpretTime(value: string): string {
    const regexResult = this.regex.exec(value);

    if (!regexResult || !regexResult.groups)
      return 'Invalid syntax';

    let result = 'now';

    const addSign = regexResult.groups['ASIGN'];
    const addValue = regexResult.groups['AVALUE'];
    const addUnit = regexResult.groups['AUNIT'];

    if (addSign && addValue && addUnit) {
      result = `${addValue} ${this.timeAbbreviations[addUnit]}${addValue !== '1' ? 's' : ''}`;
      result = `${result} ${addSign === '+' ? 'from now' : 'ago'}`;
    }

    const roundType = regexResult.groups['RTYPE'];
    const roundUnit = regexResult.groups['RUNIT'];

    if (roundType && roundUnit) {
      result = `the ${roundType === 's' ? 'start' : 'end'} of the ${this.timeAbbreviations[roundUnit]} ${result}`;
    }

    return result;
  }

  trySubmit() {
    if (!this.timeStartForm)
      return;

    if (!this.timeEndForm)
      return;

    const timeStartValue = this.timeStartForm.value;
    if (this.interpretTime(timeStartValue) === 'Invalid syntax')
      return;

    const timeEndValue = this.timeEndForm.value;
    if (this.interpretTime(timeEndValue) === 'Invalid syntax')
      return;

    const result: TimeConfiguration = {
      startTime: timeStartValue,
      endTime: timeEndValue,
    };
    this.$activeModal.close(result);
  }
}

export interface TimeConfiguration {
  startTime: string;
  endTime: string;
}
