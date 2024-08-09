import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MonitoredEnvironmentDetailComponent } from './monitored-environment-detail.component';

describe('MonitoredEnvironmentDetailComponent', () => {
  let component: MonitoredEnvironmentDetailComponent;
  let fixture: ComponentFixture<MonitoredEnvironmentDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MonitoredEnvironmentDetailComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MonitoredEnvironmentDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
