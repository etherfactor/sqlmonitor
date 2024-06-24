import { TestBed } from '@angular/core/testing';
import { InstanceService } from './instance.service';
import { provideInstanceService } from './instance.service.concrete';

describe('InstanceService', () => {
  let service: InstanceService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideInstanceService(),
      ]
    });
    service = TestBed.inject(InstanceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
