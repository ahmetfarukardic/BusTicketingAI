import { CommonModule } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BoxOfficeService } from '../../../services/box-office.service';
import { AlertService } from '../../../services/alert.service';
import { CompanyActiveTrip, SellTicketRequest, Terminal } from '../../../core/models/box-office.model';
import { FlatpickrDirective } from '../../../directives/flatpickr.directive';

@Component({
  selector: 'app-box-office',
  standalone: true,
  imports: [CommonModule, FormsModule, FlatpickrDirective],
  templateUrl: './box-office.html',
  styleUrl: './box-office.scss',
})
export class BoxOffice implements OnInit {
  private boxOfficeService = inject(BoxOfficeService);
  private alertService = inject(AlertService);

  terminals = signal<Terminal[]>([]);
  trips = signal<CompanyActiveTrip[]>([]);
  selectedTrip = signal<CompanyActiveTrip | null>(null);
  occupiedSeats = signal<number[]>([]);

  filterOrigin = signal<number | null>(null);
  filterDest = signal<number | null>(null);
  filterDate = signal<string>('');
  
  selectedSeat = signal<number | null>(null);
  sellForm = signal({ passengerName: '', passengerTC: '' });

  searchText = signal<string>('');

  ngOnInit(): void {
    this.loadTrips();
    this.loadTerminals();
  }

  loadTerminals() {
    this.boxOfficeService.getTerminals().subscribe({
      next: (data) => this.terminals.set(data),
      error: () => this.alertService.error('Hata', 'Terminaller yüklenemedi.')
    });
  }

  loadTrips() {
    this.boxOfficeService.getActiveTrips(this.filterOrigin(), this.filterDest(), this.filterDate()).subscribe({
      next: (data) => {
        this.trips.set(data);
        this.selectedTrip.set(null);
      },
      error: () => this.alertService.error('Hata!', 'Seferler yüklenemedi.')
    });
  }

  clearFilters() {
    this.filterOrigin.set(null);
    this.filterDest.set(null);
    this.filterDate.set('');
    this.loadTrips();
  }

  swapCities() {
    const origin = this.filterOrigin();
    const dest = this.filterDest();
    this.filterOrigin.set(dest);
    this.filterDest.set(origin);
  }

  selectTrip(trip: CompanyActiveTrip) {
    this.selectedTrip.set(trip);
    this.selectedSeat.set(null); 
    this.loadOccupiedSeats(trip.tripId);
  }

  loadOccupiedSeats(tripId: string) {
    this.boxOfficeService.getOccupiedSeats(tripId).subscribe({
      next: (data) => this.occupiedSeats.set(data),
      error: () => this.alertService.error('Hata!', 'Koltuk durumu yüklenemedi.')
    });
  }

  selectSeat(seatNo: number) {
    if (this.occupiedSeats().includes(seatNo)) return; 
    
    if (this.selectedSeat() === seatNo) {
      this.selectedSeat.set(null);
    } else {
      this.selectedSeat.set(seatNo);
      this.sellForm.set({ passengerName: '', passengerTC: '' }); 
    }
  }

  getSeatArray(totalSeats: number): number[] {
    return Array.from({ length: totalSeats }, (_, i) => i + 1);
  }

  updateForm(field: 'passengerName' | 'passengerTC', value: string) {
    this.sellForm.update(currentForm => ({
      ...currentForm,
      [field]: value
    }));
  }

  sellTicket() {
    const trip = this.selectedTrip();
    const seat = this.selectedSeat();
    const form = this.sellForm();

    if (!trip || !seat || !form.passengerName || !form.passengerTC) {
      this.alertService.error('Uyarı', 'Lütfen tüm alanları doldurun.');
      return;
    }

    const payload: SellTicketRequest = {
      tripId: trip.tripId,
      seatNumber: seat,
      passengerName: form.passengerName,
      passengerTC: form.passengerTC,
      price: trip.price
    };

    this.boxOfficeService.sellTicket(payload).subscribe({
      next: (res) => {
        this.alertService.success('Başarılı!', res.message);
        this.selectedSeat.set(null);
        this.sellForm.set({ passengerName: '', passengerTC: '' });
        this.loadOccupiedSeats(trip.tripId);
      },
      error: () => this.alertService.error('Hata!', 'Bilet satışı gerçekleştirilemedi.')
    });
  }
}
