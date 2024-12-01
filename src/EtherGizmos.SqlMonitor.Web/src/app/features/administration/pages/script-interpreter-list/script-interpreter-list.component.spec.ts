import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScriptInterpreterListComponent } from './script-interpreter-list.component';

describe('ScriptInterpreterListComponent', () => {
  let component: ScriptInterpreterListComponent;
  let fixture: ComponentFixture<ScriptInterpreterListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ScriptInterpreterListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ScriptInterpreterListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
