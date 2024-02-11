import { ElementRef, Provider } from "@angular/core";

class MockElementRef extends ElementRef {

  constructor(element?: unknown) {
    super(element ?? {});
  }
}

export function provideElementRefMock(element?: unknown): Provider {
  return {
    provide: ElementRef,
    useFactory: () => new MockElementRef(element),
  };
}
