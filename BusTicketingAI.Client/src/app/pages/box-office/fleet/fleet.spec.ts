import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Fleet } from './fleet';

describe('Fleet', () => {
  let component: Fleet;
  let fixture: ComponentFixture<Fleet>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Fleet]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Fleet);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
