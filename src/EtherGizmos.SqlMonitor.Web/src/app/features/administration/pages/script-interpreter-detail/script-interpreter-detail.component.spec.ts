import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScriptInterpreterDetailComponent } from './script-interpreter-detail.component';

describe('ScriptInterpreterDetailComponent', () => {
  let component: ScriptInterpreterDetailComponent;
  let fixture: ComponentFixture<ScriptInterpreterDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ScriptInterpreterDetailComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ScriptInterpreterDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
