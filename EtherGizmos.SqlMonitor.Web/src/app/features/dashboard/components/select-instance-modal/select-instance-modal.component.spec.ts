import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { provideInstanceServiceMock } from '../../../../shared/services/instance/instance.service.mock';
import { parseGuid } from '../../../../shared/types/guid/guid';
import { SelectInstanceModalComponent } from './select-instance-modal.component';

describe('SelectInstanceModalComponent', () => {
  let component: SelectInstanceModalComponent;
  let fixture: ComponentFixture<SelectInstanceModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        SelectInstanceModalComponent,
      ],
      providers: [
        NgbActiveModal,
        provideInstanceServiceMock(),
      ]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SelectInstanceModalComponent);
    component = fixture.componentInstance;
    component.setInstanceIds([
      parseGuid('006db56a-d3b1-4c50-af8f-f19bb13f85ed'),
    ]);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load instances', () => {
    const instanceCheckboxes = fixture.debugElement.queryAll(By.css('[data-testid="select-instance-checkbox"]'));
    expect(instanceCheckboxes.length).toBe(3);
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

  it('should call close when form is valid and ok is clicked', () => {
    const instanceCheckboxes = fixture.debugElement.queryAll(By.css('[data-testid="select-instance-checkbox"]'));
    expect(instanceCheckboxes.length).toBe(3);

    instanceCheckboxes[1].nativeElement.click();
    fixture.detectChanges();

    const okButton = fixture.debugElement.query(By.css('[data-testid="ok-button"]'));
    expect(okButton).toBeTruthy();

    const $activeModal = TestBed.inject(NgbActiveModal);
    const dismissSpy = spyOn($activeModal, 'dismiss');
    const closeSpy = spyOn($activeModal, 'close');

    okButton.nativeElement.click();
    fixture.detectChanges();

    expect(dismissSpy).not.toHaveBeenCalled();
    expect(closeSpy).toHaveBeenCalledWith(['006db56a-d3b1-4c50-af8f-f19bb13f85ed', '5d469831-4c01-487d-a19b-78bdd6c99798']);
  });
});
