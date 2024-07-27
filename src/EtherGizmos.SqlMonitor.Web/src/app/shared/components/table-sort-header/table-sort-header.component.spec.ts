import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TableSortHeaderComponent } from './table-sort-header.component';

describe('TableSortHeaderComponent', () => {
  let component: TableSortHeaderComponent;
  let fixture: ComponentFixture<TableSortHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TableSortHeaderComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TableSortHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
