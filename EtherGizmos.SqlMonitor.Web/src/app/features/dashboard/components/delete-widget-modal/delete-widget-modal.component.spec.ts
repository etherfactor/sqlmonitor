import { ComponentFixture, TestBed } from '@angular/core/testing';
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
});
