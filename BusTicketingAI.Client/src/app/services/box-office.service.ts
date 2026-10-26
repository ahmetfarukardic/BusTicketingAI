import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CompanyActiveTrip, SellTicketRequest, Terminal } from '../core/models/box-office.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class BoxOfficeService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/companystaff`;
  private terminalsUrl = `${environment.apiUrl}/terminals`;
  private ticketsApiUrl = `${environment.apiUrl}/tickets`;

  getTerminals(): Observable<Terminal[]> {
    return this.http.get<Terminal[]>(this.terminalsUrl)
  }

  getActiveTrips(originId?: number | null, destId?: number | null, date?: string | null): Observable<CompanyActiveTrip[]> {
    let params = new HttpParams();

    if (originId) params = params.set('originId', originId.toString());
    if (destId) params = params.set('destinationId', destId.toString());
    if (date) params = params.set('date', date.toString());

    return this.http.get<CompanyActiveTrip[]>(`${this.apiUrl}/active-trips`, { params });
  }

  getOccupiedSeats(tripId: string): Observable<{ seatNumber: number, gender: 'E' | 'K' }[]> {
    return this.http.get<{ seatNumber: number, gender: 'E' | 'K' }[]>(`${this.apiUrl}/trips/${tripId}/occupied-seats`)
  }

  sellTicket(payload: SellTicketRequest): Observable<{ id: string, message: string }> {
    return this.http.post<{ id: string, message: string }>(`${this.ticketsApiUrl}/sell`, payload);
  }
}
