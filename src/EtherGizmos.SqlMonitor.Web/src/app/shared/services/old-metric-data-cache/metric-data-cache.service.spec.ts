import { TestBed } from '@angular/core/testing';

import { MetricDataCacheService } from './metric-data-cache.service';

describe('MetricDataCacheService', () => {
  let service: MetricDataCacheService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MetricDataCacheService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
