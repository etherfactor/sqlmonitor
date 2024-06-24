import { TestBed } from '@angular/core/testing';

import { NavbarMenuService } from './navbar-menu.service';

describe('NavbarMenuService', () => {
  let service: NavbarMenuService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(NavbarMenuService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
