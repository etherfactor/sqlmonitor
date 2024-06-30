import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MonitoredSystemListComponent } from './monitored-system-list.component';

describe('MonitoredSystemListComponent', () => {
  let component: MonitoredSystemListComponent;
  let fixture: ComponentFixture<MonitoredSystemListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MonitoredSystemListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MonitoredSystemListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
