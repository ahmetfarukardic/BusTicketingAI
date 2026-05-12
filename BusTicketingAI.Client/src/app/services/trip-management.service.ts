import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Bus, CreateTripRequest, UpdateTripTimeRequest } from '../core/models/bus';
import { CreateBusRequest } from '../core/models/create-bus-request';
import { Passenger } from '../core/models/passenger';

@Injectable({
  providedIn: 'root',
})
export class TripManagementService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7202/api/companystaff';

  getCompanyBuses(): Observable<Bus[]> {
    return this.http.get<Bus[]>(`${this.apiUrl}/buses`);
  }

  createBus(payload: CreateBusRequest): Observable<{ busId: number, message: string }> {
    return this.http.post<{ busId: number, message: string }>(`${this.apiUrl}/buses`, payload);
  }

  deleteBus(busId: number):Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.apiUrl}/buses/${busId}`);
  }

  createTrip(payload: CreateTripRequest): Observable<{ tripId: string, message: string }> {
    return this.http.post<{ tripId: string, message: string }>(`${this.apiUrl}/trips`, payload);
  }

  updateTripTime(tripId: string, payload: UpdateTripTimeRequest): Observable<{ message: string }> {
    return this.http.patch<{ message: string}>(`${this.apiUrl}/trips/${tripId}/time`, payload);
  } 

  getTripPassengers(tripId: string): Observable<Passenger[]> {
    return this.http.get<Passenger[]>(`${this.apiUrl}/trips/${tripId}/passengers`);
  }

  cancelTicket(ticketId: string): Observable<{ message: string}> {
    return this.http.patch<{ message: string}>(`${this.apiUrl}/tickets/${ticketId}/cancel`, {});
  }
}
