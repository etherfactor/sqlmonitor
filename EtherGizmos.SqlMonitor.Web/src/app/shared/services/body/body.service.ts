import { Injectable } from '@angular/core';
import { BehaviorSubject, asyncScheduler, observeOn } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BodyService {

  private containerSubject = new BehaviorSubject<string>('container');

  constructor() { }

  setContainer(containerType: BodyContainerType) {
    switch (containerType) {
      case (BodyContainerType.Fluid):
      case (BodyContainerType.Normal):
        this.containerSubject.next(containerType);
        break;

      default:
        throw new Error(`Unrecognized container type ${containerType}`);
    }
  }

  get containerClass$() {
    return this.containerSubject.pipe(
      observeOn(asyncScheduler)
    );
  }
}

export enum BodyContainerType {
  Normal = 'container',
  Fluid = 'container-fluid',
}
