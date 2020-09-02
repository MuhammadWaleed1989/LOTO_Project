import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminConfigComponent } from './adminconfig.component';

describe('CustomersComponent', () => {
  let component: AdminConfigComponent;
  let fixture: ComponentFixture<AdminConfigComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [AdminConfigComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdminConfigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
