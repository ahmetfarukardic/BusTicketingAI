import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BusCompanies } from './bus-companies';

describe('BusCompanies', () => {
  let component: BusCompanies;
  let fixture: ComponentFixture<BusCompanies>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BusCompanies]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BusCompanies);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
