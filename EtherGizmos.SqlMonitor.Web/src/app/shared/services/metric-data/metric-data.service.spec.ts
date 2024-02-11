import { TestBed } from '@angular/core/testing';
import { MetricDataService } from './metric-data.service';
import { provideMetricDataService } from './metric-data.service.concrete';

describe('MetricDataService', () => {
  let service: MetricDataService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideMetricDataService(),
      ]
    });
    service = TestBed.inject(MetricDataService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
