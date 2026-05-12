import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class BusCompanyService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7202/api/buscompanies'

  getAll(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  create(company: {name: string}): Observable<any> {
    return this.http.post(this.apiUrl, company)
  }

  update(id: number, company: {id: number, name: string}): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, company)
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
