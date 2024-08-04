import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MonitoredSystemDetailComponent } from './monitored-system-detail.component';

describe('MonitoredSystemDetailComponent', () => {
  let component: MonitoredSystemDetailComponent;
  let fixture: ComponentFixture<MonitoredSystemDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MonitoredSystemDetailComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MonitoredSystemDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
