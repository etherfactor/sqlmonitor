import { TestBed } from '@angular/core/testing';

import { ScriptInterpreterService } from './script-interpreter.service';

describe('ScriptInterpreterService', () => {
  let service: ScriptInterpreterService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ScriptInterpreterService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
