import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeleteWidgetModalComponent } from './delete-widget-modal.component';

describe('DeleteWidgetModalComponent', () => {
  let component: DeleteWidgetModalComponent;
  let fixture: ComponentFixture<DeleteWidgetModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeleteWidgetModalComponent]
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
