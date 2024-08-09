import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MonitoredResourceListComponent } from './monitored-resource-list.component';

describe('MonitoredResourceListComponent', () => {
  let component: MonitoredResourceListComponent;
  let fixture: ComponentFixture<MonitoredResourceListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MonitoredResourceListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MonitoredResourceListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
