import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FilterBuilderGroupComponent } from './filter-builder-group.component';

describe('FilterBuilderGroupComponent', () => {
  let component: FilterBuilderGroupComponent;
  let fixture: ComponentFixture<FilterBuilderGroupComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FilterBuilderGroupComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FilterBuilderGroupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
