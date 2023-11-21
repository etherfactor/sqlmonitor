import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, asapScheduler, observeOn } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NavbarMenuService {
  
  private actionsSubject = new BehaviorSubject<NavbarMenuAction[]>([]);
  private breadcrumbsSubject = new BehaviorSubject<NavbarMenuBreadcrumb[]>([]);

  constructor() { }
  
  setActions(actions: NavbarMenuAction[]) {
    this.actionsSubject.next(actions);
  }

  setBreadcrumbs(breadcrumbs: NavbarMenuBreadcrumb[]) {
    this.breadcrumbsSubject.next(breadcrumbs);
  }

  get actions$(): Observable<NavbarMenuAction[]> {
    return this.actionsSubject.pipe(
      observeOn(asapScheduler)
    );
  }

  get breadcrumbs$(): Observable<NavbarMenuBreadcrumb[]> {
    return this.breadcrumbsSubject.pipe(
      observeOn(asapScheduler)
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

export interface NavbarMenuBreadcrumb {
  label: string;
  link: string;
}
