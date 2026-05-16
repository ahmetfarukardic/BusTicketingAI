import { CommonModule } from '@angular/common';
import { Component, ElementRef, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TripService } from '../../services/trip.service';
import { FlatpickrDirective } from '../../directives/flatpickr.directive';


interface Trip {
  id: string;
  from: string;
  to: string;
  date: string;
  time: string;
  arrivalTime?: string;
  price: number;
  availableSeats: number;
  companyName?: string;
}

@Component({
  selector: 'app-trips',
  standalone: true,
  imports: [CommonModule, FormsModule, FlatpickrDirective],
  templateUrl: './trips.html',
  styleUrl: './trips.scss',
})
export class Trips implements OnInit{
  private router = inject(Router);
  private tripService = inject(TripService);

  tripList: Trip[] = [];

  cities: {id: number, name: string, } [] = [];

  searchOriginId: number | undefined;
  searchDestinationId: number | undefined;
  searchDate: string | undefined;
  minDate: string = '';

  ngOnInit() {
    const today = new Date();
    const year = today.getFullYear();
    const month = String(today.getMonth() + 1).padStart(2, '0');
    const day = String(today.getDate()).padStart(2, '0');
    this.minDate = `${year}-${month}-${day}`;
    this.searchDate = this.minDate;

    this.loadCities();
    this.onSearch();
  }

  loadCities() {
    this.tripService.getCities().subscribe({
      next: (data) => {
        this.cities = data;
      }, 
      error: (err) => console.error('Şehirler yüklenirken hata oluştu: ', err)
    });
  }

  onSearch() {
    this.tripService.searchTrips(this.searchOriginId, this.searchDestinationId, this.searchDate)
    .subscribe({
      next: (response: any[]) => {
        this.tripList = response.map(item => {
          const departureDate = new Date(item.departureTime);
          let arrivalTimeStr = '--:--';

          if (item.estimatedDuration) {
            const arrivalDate = new Date(departureDate.getTime());

            if (typeof item.estimatedDuration === 'string') {
              const parts = item.estimatedDuration.split(':');
              if (parts.length >= 2) {
                arrivalDate.setHours(arrivalDate.getHours() + parseInt(parts[0], 10));
                arrivalDate.setMinutes(arrivalDate.getMinutes() + parseInt(parts[1], 10));
              }
            }
            else if (typeof item.estimatedDuration === 'number') {
              arrivalDate.setMinutes(arrivalDate.getMinutes() + item.estimatedDuration);
            }
            arrivalTimeStr = arrivalDate.toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' });
          }
          return {
            id: item.tripId, 
            from: item.originTerminal,
            to: item.destinationTerminal,
            date: item.departureTime,
            time: departureDate.toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' }),
            arrivalTime: arrivalTimeStr,
            price: item.price,
            availableSeats: item.emptySeats,
            companyName: item.companyName
          };
        });
      },
      error: (err) => console.error('Seferler Cekilemedi.', err)
    });
  }

  onTripSelect(tripId: string) {
    const token = localStorage.getItem('token');
    if (token) {
      this.router.navigate([`/trip/${tripId}/seats`]);
    } else {
      this.router.navigate(['/login'], {queryParams: {returnUrl: `/trip/${tripId}/seats`} });
    }
  }

  swapCities() {
    const tempOrigin = this.searchOriginId;
    this.searchOriginId = this.searchDestinationId;
    this.searchDestinationId = tempOrigin;
  }
}