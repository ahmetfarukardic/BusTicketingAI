import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PassengerTicketResponseDto } from '../core/models/ticket-model';
import { AnyCatcher } from 'rxjs/internal/AnyCatcher';

@Injectable({
  providedIn: 'root',
})
export class ProfileService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7202/api/profile';

  getMyTickets(): Observable<PassengerTicketResponseDto> {
    return this.http.get<PassengerTicketResponseDto>(this.apiUrl);
  }

  getMyProfile(): Observable<any> {
    return this.http.get(`${this.apiUrl}/me`);
  }

  checkoutTicket(payload: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/checkout`, payload);
  }

  cancelMyTicket(ticketId: string): Observable<any> {
    return this.http.patch(`${this.apiUrl}/profile/${ticketId}/cancel`, {});
  }

  lockSeats(payload: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/lock-seats`, payload);
  }

  unlockSeats(payload: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/unlock-seats`, payload);
  }
}
