import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, asyncScheduler, observeOn } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NavbarMenuService {
  
  private actionsSubject = new BehaviorSubject<NavbarMenuAction[]>([]);

  constructor() { }
  
  setActions(actions: NavbarMenuAction[]) {
    this.actionsSubject.next(actions);
  }

  get actions$(): Observable<NavbarMenuAction[]> {
    return this.actionsSubject.pipe(
      observeOn(asyncScheduler)
    );
  }
}

export interface NavbarMenuCallback {
  callback?: () => any;
}

export interface NavbarMenuAction extends NavbarMenuCallback {
  label: string;
  icon?: string;
  subActions?: NavbarMenuSubAction[];
}

export interface NavbarMenuSubAction extends NavbarMenuCallback {
  label?: string;
  icon?: string;
  divider?: boolean;
}
