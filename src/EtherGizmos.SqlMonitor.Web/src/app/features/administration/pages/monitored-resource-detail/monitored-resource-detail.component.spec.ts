import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MonitoredResourceDetailComponent } from './monitored-resource-detail.component';

describe('MonitoredResourceDetailComponent', () => {
  let component: MonitoredResourceDetailComponent;
  let fixture: ComponentFixture<MonitoredResourceDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MonitoredResourceDetailComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MonitoredResourceDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
