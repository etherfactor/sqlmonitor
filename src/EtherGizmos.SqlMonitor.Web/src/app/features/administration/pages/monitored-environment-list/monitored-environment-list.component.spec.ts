import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MonitoredEnvironmentListComponent } from './monitored-environment-list.component';

describe('MonitoredEnvironmentListComponent', () => {
  let component: MonitoredEnvironmentListComponent;
  let fixture: ComponentFixture<MonitoredEnvironmentListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MonitoredEnvironmentListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MonitoredEnvironmentListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
