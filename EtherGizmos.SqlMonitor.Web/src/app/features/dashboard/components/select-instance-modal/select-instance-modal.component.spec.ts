import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { provideInstanceServiceMock } from '../../../../shared/services/instance/instance.service.mock';
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
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
