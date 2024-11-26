import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScriptDetailComponent } from './script-detail.component';

describe('ScriptDetailComponent', () => {
  let component: ScriptDetailComponent;
  let fixture: ComponentFixture<ScriptDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ScriptDetailComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ScriptDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
