import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { DeleteWidgetModalComponent } from './delete-widget-modal.component';

describe('DeleteWidgetModalComponent', () => {
  let component: DeleteWidgetModalComponent;
  let fixture: ComponentFixture<DeleteWidgetModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        DeleteWidgetModalComponent,
      ],
      providers: [
        NgbActiveModal,
      ]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DeleteWidgetModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
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

  it('should call close when delete is clicked', () => {
    const deleteButton = fixture.debugElement.query(By.css('[data-testid="delete-button"]'));
    expect(deleteButton).toBeTruthy();

    const $activeModal = TestBed.inject(NgbActiveModal);
    const dismissSpy = spyOn($activeModal, 'dismiss');
    const closeSpy = spyOn($activeModal, 'close');

    deleteButton.nativeElement.click();
    fixture.detectChanges();

    expect(dismissSpy).not.toHaveBeenCalled();
    expect(closeSpy).toHaveBeenCalled();
  });
});
