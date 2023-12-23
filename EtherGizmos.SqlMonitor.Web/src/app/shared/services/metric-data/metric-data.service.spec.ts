import { TestBed } from '@angular/core/testing';

import { MetricDataService } from './metric-data.service';

describe('MetricDataService', () => {
  let service: MetricDataService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MetricDataService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
