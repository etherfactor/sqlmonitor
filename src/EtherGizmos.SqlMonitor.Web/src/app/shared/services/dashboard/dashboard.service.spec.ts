import { TestBed } from '@angular/core/testing';
import { DashboardService } from './dashboard.service';
import { provideDashboardService } from './dashboard.service.concrete';

describe('DashboardService', () => {
  let service: DashboardService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideDashboardService(),
      ]
    });
    service = TestBed.inject(DashboardService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
