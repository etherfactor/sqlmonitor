import { ElementRef, Provider } from "@angular/core";

class MockElementRef extends ElementRef {

  constructor() {
    super({});
  }
}

export function provideMockElementRef(): Provider {
  return {
    provide: ElementRef,
    useFactory: () => new MockElementRef(),
  };
}
