import { TestBed } from '@angular/core/testing';

import { MonitoredEnvironmentService } from './monitored-environment.service';

describe('MonitoredEnvironmentService', () => {
  let service: MonitoredEnvironmentService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MonitoredEnvironmentService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
