import { TestBed } from '@angular/core/testing';

import { TimeWindowService } from './time-window.service';

describe('TimeWindowService', () => {
  let service: TimeWindowService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TimeWindowService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
