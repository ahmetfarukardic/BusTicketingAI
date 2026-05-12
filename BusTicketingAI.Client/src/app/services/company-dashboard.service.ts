import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CompanyDailyStats } from '../core/models/company-daily-stats';

@Injectable({
  providedIn: 'root',
})
export class CompanyDashboardService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7202/api/companystaff';

  getCompanyDailyStats(companyId: number, date: string): Observable<CompanyDailyStats> {
    const params = new HttpParams().set('date', date);

    return this.http.get<CompanyDailyStats>(`${this.apiUrl}`, { params });
  }
}
