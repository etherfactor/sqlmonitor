import { TestBed } from '@angular/core/testing';

import { MonitoredResourceService } from './monitored-resource.service';

describe('MonitoredResourceService', () => {
  let service: MonitoredResourceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MonitoredResourceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
