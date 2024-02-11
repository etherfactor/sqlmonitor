import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { SelectTimeModalComponent } from './select-time-modal.component';

describe('SelectTimeModalComponent', () => {
  let component: SelectTimeModalComponent;
  let fixture: ComponentFixture<SelectTimeModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        SelectTimeModalComponent,
      ],
      providers: [
        NgbActiveModal,
      ]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SelectTimeModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
