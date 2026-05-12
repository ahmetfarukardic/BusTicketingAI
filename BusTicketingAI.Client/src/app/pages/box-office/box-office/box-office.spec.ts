import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BoxOffice } from './box-office';

describe('BoxOffice', () => {
  let component: BoxOffice;
  let fixture: ComponentFixture<BoxOffice>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BoxOffice]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BoxOffice);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
