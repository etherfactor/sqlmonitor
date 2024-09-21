import { CommonModule } from '@angular/common';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { By } from '@angular/platform-browser';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { parseRelativeTime } from '../../../../shared/types/relative-time/relative-time';
import { SelectTimeModalComponent } from './select-time-modal.component';

describe('SelectTimeModalComponent', () => {
  let component: SelectTimeModalComponent;
  let fixture: ComponentFixture<SelectTimeModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        CommonModule,
        ReactiveFormsModule,
        SelectTimeModalComponent,
      ],
      providers: [
        NgbActiveModal,
      ]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SelectTimeModalComponent);
    component = fixture.componentInstance;

    component.setTime({
      startTime: parseRelativeTime('t-5m'),
      endTime: parseRelativeTime('t'),
    });
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should populate inputs based on form', () => {
    const startInput = fixture.debugElement.query(By.css('[data-testid="start-input"]'));
    expect(startInput).toBeTruthy();

    expect(startInput.nativeElement.value).toBe('t-5m');

    const endInput = fixture.debugElement.query(By.css('[data-testid="end-input"]'));
    expect(endInput).toBeTruthy();

    expect(endInput.nativeElement.value).toBe('t');
  });

  it('should call dismiss when x is clicked', () => {
    const xButton = fixture.debugElement.query(By.css('[data-testid="x-button"]'));
    expect(xButton).toBeTruthy();

    const $activeModal = TestBed.inject(NgbActiveModal);
    const dismissSpy = spyOn($activeModal, 'dismiss');
    const closeSpy = spyOn($activeModal, 'close');

    xButton.nativeElement.click();
    fixture.detectChanges();

    expect(dismissSpy).toHaveBeenCalled();
    expect(closeSpy).not.toHaveBeenCalled();
  });

  it('should call dismiss when cancel is clicked', () => {
    const cancelButton = fixture.debugElement.query(By.css('[data-testid="cancel-button"]'));
    expect(cancelButton).toBeTruthy();

    const $activeModal = TestBed.inject(NgbActiveModal);
    const dismissSpy = spyOn($activeModal, 'dismiss');
    const closeSpy = spyOn($activeModal, 'close');

    cancelButton.nativeElement.click();
    fixture.detectChanges();

    expect(dismissSpy).toHaveBeenCalled();
    expect(closeSpy).not.toHaveBeenCalled();
  });

  it('should not call close when form is not valid and ok is clicked', () => {
    component.timeStartForm?.setValue('t-5');
    component.timeStartForm?.updateValueAndValidity();

    fixture.detectChanges();

    expect(component.timeStartForm?.valid).toBeFalse();
    expect(component.timeEndForm?.valid).toBeTrue();

    const okButton = fixture.debugElement.query(By.css('[data-testid="ok-button"]'));
    expect(okButton).toBeTruthy();

    const $activeModal = TestBed.inject(NgbActiveModal);
    const dismissSpy = spyOn($activeModal, 'dismiss');
    const closeSpy = spyOn($activeModal, 'close');

    okButton.nativeElement.click();
    fixture.detectChanges();

    expect(dismissSpy).not.toHaveBeenCalled();
    expect(closeSpy).not.toHaveBeenCalled();
  });

  it('should not call close when start time is later than end time', () => {
    component.timeStartForm?.setValue('t');
    component.timeStartForm?.updateValueAndValidity();

    component.timeEndForm?.setValue('t-5m');
    component.timeEndForm?.updateValueAndValidity();

    fixture.detectChanges();

    expect(component.timeStartForm?.valid).toBeFalse();
    expect(component.timeEndForm?.valid).toBeTrue();

    const okButton = fixture.debugElement.query(By.css('[data-testid="ok-button"]'));
    expect(okButton).toBeTruthy();

    const $activeModal = TestBed.inject(NgbActiveModal);
    const dismissSpy = spyOn($activeModal, 'dismiss');
    const closeSpy = spyOn($activeModal, 'close');

    okButton.nativeElement.click();
    fixture.detectChanges();

    expect(dismissSpy).not.toHaveBeenCalled();
    expect(closeSpy).not.toHaveBeenCalled();
  });

  it('should not call close when start time is the same as end time', () => {
    component.timeStartForm?.setValue('t');
    component.timeStartForm?.updateValueAndValidity();

    component.timeEndForm?.setValue('t');
    component.timeEndForm?.updateValueAndValidity();

    fixture.detectChanges();

    expect(component.timeStartForm?.valid).toBeFalse();
    expect(component.timeEndForm?.valid).toBeTrue();

    const okButton = fixture.debugElement.query(By.css('[data-testid="ok-button"]'));
    expect(okButton).toBeTruthy();

    const $activeModal = TestBed.inject(NgbActiveModal);
    const dismissSpy = spyOn($activeModal, 'dismiss');
    const closeSpy = spyOn($activeModal, 'close');

    okButton.nativeElement.click();
    fixture.detectChanges();

    expect(dismissSpy).not.toHaveBeenCalled();
    expect(closeSpy).not.toHaveBeenCalled();
  });

  it('should call close when form is valid and ok is clicked', () => {
    const startInput = fixture.debugElement.query(By.css('[data-testid="start-input"]'));
    expect(startInput).toBeTruthy();

    const endInput = fixture.debugElement.query(By.css('[data-testid="end-input"]'));
    expect(endInput).toBeTruthy();

    startInput.nativeElement.value = "t-5m";
    fixture.detectChanges();

    endInput.nativeElement.value = "t";
    fixture.detectChanges();

    const okButton = fixture.debugElement.query(By.css('[data-testid="ok-button"]'));
    expect(okButton).toBeTruthy();

    const $activeModal = TestBed.inject(NgbActiveModal);
    const dismissSpy = spyOn($activeModal, 'dismiss');
    const closeSpy = spyOn($activeModal, 'close');

    okButton.nativeElement.click();
    fixture.detectChanges();

    expect(dismissSpy).not.toHaveBeenCalled();
    expect(closeSpy).toHaveBeenCalledWith({ startTime: 't-5m', endTime: 't' });
  });
});
