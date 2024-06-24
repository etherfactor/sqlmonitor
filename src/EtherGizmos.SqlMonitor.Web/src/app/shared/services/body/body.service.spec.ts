import { TestBed, fakeAsync, tick } from '@angular/core/testing';
import { BodyContainerType, BodyService } from './body.service';

describe('BodyService', () => {
  let service: BodyService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BodyService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should emit when setContainer is called', fakeAsync(() => {
    let emittedClass: string | undefined;

    service.containerClass$.subscribe(value => {
      emittedClass = value;
    });

    service.setContainer(BodyContainerType.Normal);
    tick();

    expect(emittedClass).toBe('container');

    service.setContainer(BodyContainerType.Fluid);
    tick();

    expect(emittedClass).toBe('container-fluid');
  }));
});
