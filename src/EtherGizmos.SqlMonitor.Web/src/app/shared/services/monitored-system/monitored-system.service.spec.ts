import { TestBed } from '@angular/core/testing';

import { MonitoredSystemService } from './monitored-system.service';

describe('MonitoredSystemService', () => {
  let service: MonitoredSystemService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MonitoredSystemService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
