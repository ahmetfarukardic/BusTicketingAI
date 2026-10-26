import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CompanyDailyStats } from '../core/models/company-daily-stats';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class CompanyDashboardService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/companystaff`;

  getCompanyDailyStats(companyId: number, date: string): Observable<CompanyDailyStats> {
    const params = new HttpParams().set('date', date);

    return this.http.get<CompanyDailyStats>(`${this.apiUrl}`, { params });
  }
}
