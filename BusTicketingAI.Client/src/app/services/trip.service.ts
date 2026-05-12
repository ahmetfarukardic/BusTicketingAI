import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class TripService {
  private http = inject(HttpClient);

  private apiUrl = 'https://localhost:7202/api';

  searchTrips(originId?: number, destinationId?: number, date?: string) : Observable<any[]> {
    let params = new HttpParams();
    if (originId) params = params.set('OriginCityId', originId);
    if (destinationId) params = params.set('DestinationCityId', destinationId);
    if (date) params = params.set('DepartureDate', date);

    return this.http.get<any[]>(`${this.apiUrl}/trips/search`, {params});
  }

  getCities(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/cities`)
  }

  getTripDetails(tripId: string): Observable<any>{
    return this.http.get<any>(`${this.apiUrl}/trips/${tripId}/details`);
  }
}
