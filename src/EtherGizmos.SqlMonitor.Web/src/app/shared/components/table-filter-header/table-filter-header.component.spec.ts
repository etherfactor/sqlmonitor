import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TableFilterHeaderComponent } from './table-filter-header.component';

describe('TableFilterHeaderComponent', () => {
  let component: TableFilterHeaderComponent;
  let fixture: ComponentFixture<TableFilterHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TableFilterHeaderComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TableFilterHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
