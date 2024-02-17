import { TestBed } from '@angular/core/testing';
import { MetricService } from './metric.service';
import { provideMetricService } from './metric.service.concrete';

describe('MetricService', () => {
  let service: MetricService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideMetricService(),
      ]
    });
    service = TestBed.inject(MetricService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
